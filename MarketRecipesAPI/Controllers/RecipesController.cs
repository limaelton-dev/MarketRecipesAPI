using Microsoft.AspNetCore.Mvc;
using MarketRecipesAPI.Services;
using MarketRecipesAPI.Models;
using MarketRecipesAPI.Data;
using MarketRecipesAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MarketRecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeService _recipeService;
        private readonly MarketRecipesContext _context;

        public RecipesController(RecipeService recipeService, MarketRecipesContext context)
        {
            _recipeService = recipeService;
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeCreateDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _recipeService.IsRecipeNameUnique(recipeDto.Name))
            {
                ModelState.AddModelError("Name", "O nome da receita já está em uso.");
                return BadRequest(ModelState);
            }

            if (!await _recipeService.AreIngredientsValid(recipeDto.Ingredients.Select(i => i.IngredientId).ToList()))
            {
                ModelState.AddModelError("Ingredients", "Todos os ingredientes devem ser válidos e existir no banco de dados.");
                return BadRequest(ModelState);
            }

            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                Description = recipeDto.Description,
                RecipeIngredients = recipeDto.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDto>> GetRecipeById(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeDetailDto = new RecipeDetailDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                TotalCost = recipe.TotalCost,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeDetailDto.IngredientDto
                {
                    Id = ri.Ingredient.Id,
                    Name = ri.Ingredient.Name,
                    Cost = ri.Ingredient.Cost,
                    Unit = ri.Ingredient.Unit,
                    Quantity = ri.Quantity
                }).ToList()
            };

            return Ok(recipeDetailDto);
        }

        [HttpPost("byIngredients")]
        [Authorize]
        public async Task<IActionResult> GetRecipesByIngredients([FromBody] List<int> ingredientIds)
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => ingredientIds.All(id => r.RecipeIngredients.Any(ri => ri.IngredientId == id)))
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
            return Ok(recipes);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] RecipeUpdateDto recipeUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Name = recipeUpdateDto.Name;
            recipe.Description = recipeUpdateDto.Description;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (recipeUpdateDto.Ingredients != null && recipeUpdateDto.Ingredients.Any())
                {
                    _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

                    recipe.RecipeIngredients = recipeUpdateDto.Ingredients.Select(i => new RecipeIngredient
                    {
                        IngredientId = i.IngredientId,
                        Quantity = i.Quantity,
                        RecipeId = recipe.Id
                    }).ToList();
                }

                _context.Update(recipe);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Algo de errado aconteceu ao atualizar.");
            }
        }
    }
}
