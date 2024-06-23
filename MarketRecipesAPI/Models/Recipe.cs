using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MarketRecipesAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }

        public string Description { get; set; } // Descrição da receita

        [JsonIgnore]
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        public decimal TotalCost
        {
            get
            {
                if (RecipeIngredients != null && RecipeIngredients.Any())
                {
                    return RecipeIngredients.Where(ri => ri != null && ri.Ingredient != null)
                                            .Sum(ri => ri.Ingredient.Cost * (decimal)ri.Quantity);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}