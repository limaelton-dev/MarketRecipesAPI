using MarketRecipesAPI.Data;
using MarketRecipesAPI.Dtos;
using MarketRecipesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public async Task<IActionResult> CreateIngredient([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
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

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Update(ingredient);
                await _context.SaveChangesAsync();

                // Atualiza o custo total das receitas que utilizam este ingrediente
                var recipesToUpdate = await _context.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == id))
                    .ToListAsync();

                foreach (var recipe in recipesToUpdate)
                {
                    // Atualiza o custo da receita após a mudança de custo do ingrediente
                    _context.Update(recipe);  // O Entity Framework recalcula o custo total da receita
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception (consider using a logging framework)
                return StatusCode(500, "Algo de errado aconteceu ao atualizar.");
            }
        }

    }
}
