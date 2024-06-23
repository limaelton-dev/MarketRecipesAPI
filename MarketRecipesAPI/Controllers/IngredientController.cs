using MarketRecipesAPI.Data;
using MarketRecipesAPI.Dtos;
using MarketRecipesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketRecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly MarketRecipesContext _context;

        public IngredientController(MarketRecipesContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIngredients([FromBody] IngredientsCreateDto ingredientsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredients = ingredientsDto.Ingredients.Select(i => new Ingredient
            {
                Name = i.Name,
                Cost = i.Cost,
                Unit = i.Unit
            }).ToList();

            _context.Ingredients.AddRange(ingredients);
            await _context.SaveChangesAsync();

            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIngredients()
        {
            var ingredients = await _context.Ingredients.ToListAsync();
            return Ok(ingredients);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            var recipesWithIngredient = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == id))
                .ToListAsync();

            _context.Recipes.RemoveRange(recipesWithIngredient);
            _context.Ingredients.Remove(ingredient);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            ingredient.Name = ingredientUpdateDto.Name;
            ingredient.Cost = ingredientUpdateDto.Cost;
            ingredient.Unit = ingredientUpdateDto.Unit; // Atualizando a unidade de medida

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Update(ingredient);
                await _context.SaveChangesAsync();

                var recipesToUpdate = await _context.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == id))
                    .ToListAsync();

                foreach (var recipe in recipesToUpdate)
                {
                    _context.Update(recipe);  // O Entity Framework recalcula o custo total da receita
                }

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
