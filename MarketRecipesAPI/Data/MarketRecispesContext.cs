using MarketRecipesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketRecipesAPI.Data
{
    public class MarketRecipesContext : DbContext
    {
        public MarketRecipesContext(DbContextOptions<MarketRecipesContext> options)
            : base(options) { }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Cost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
