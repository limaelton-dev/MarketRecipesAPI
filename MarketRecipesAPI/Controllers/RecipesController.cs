using Microsoft.AspNetCore.Mvc;
using MarketRecipesAPI.Services;
using MarketRecipesAPI.Models;
using MarketRecipesAPI.Data;
using MarketRecipesAPI.Dtos;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeCreateDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se o nome da receita é único
            if (!await _recipeService.IsRecipeNameUnique(recipeDto.Name))
            {
                ModelState.AddModelError("Name", "O nome da receita já está em uso.");
                return BadRequest(ModelState);
            }

            // Verifica se todos os ingredientes são válidos
            if (!await _recipeService.AreIngredientsValid(recipeDto.IngredientIds))
            {
                ModelState.AddModelError("Ingredients", "Todos os ingredientes devem ser válidos e existir no banco de dados.");
                return BadRequest(ModelState);
            }

            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                RecipeIngredients = recipeDto.IngredientIds.Select(id => new RecipeIngredient
                {
                    IngredientId = id
                }).ToList()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        [HttpPost("byIngredients")]
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
    }
}
