using MarketRecipesAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace MarketRecipesAPI.Controllers
{
    public class IngredientController : ControllerBase
    {
        private readonly MarketRecipesContext _context;

        public IngredientController(MarketRecipesContext context)
        {
            _context = context;
        }
    }
}
