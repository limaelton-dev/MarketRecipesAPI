using MarketRecipesAPI.Data;
using MarketRecipesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketRecipesAPI.Services
{
    public class RecipeService
    {
        private readonly MarketRecipesContext _context;

        public RecipeService(MarketRecipesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByIngredients(List<int> ingredientIds)
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            return recipes.Where(r => ingredientIds.All(id => r.RecipeIngredients.Any(ri => ri.IngredientId == id)))
                          .ToList();
        }

        public async Task<bool> AreIngredientsValid(List<int> ingredientIds)
        {
            foreach (var id in ingredientIds)
            {
                if (!await _context.Ingredients.AnyAsync(i => i.Id == id))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> IsRecipeNameUnique(string name)
        {
            return !await _context.Recipes.AnyAsync(r => r.Name == name);
        }
    }
}
