using MarketRecipesAPI.Data;
using MarketRecipesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MarketRecipesAPI.Services
{
    public class RecipeService
    {
        private readonly MarketRecipesContext _context;

        public RecipeService(MarketRecipesContext context)
        {
            _context = context;
        }

        public IEnumerable<Recipe> GetRecipesByIngredients(List<int> ingredientIds)
        {
            var recipes = _context.Recipes
                .Include(r => r.Ingredients)
                .ToList();

            return recipes.Where(r => ingredientIds.All(id => r.Ingredients.Any(i => i.Id == id)))
                .ToList();
        }
    }
}
