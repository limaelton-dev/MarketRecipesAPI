using MarketRecipesAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MarketRecipesAPI.Data
{
    public class MarketRecipesContext : DbContext
    {
        public MarketRecipesContext(DbContextOptions<MarketRecipesContext> options)
        : base(options) { }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
    }

}
