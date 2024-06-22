using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MarketRecipesAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }

        [JsonIgnore]
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        public decimal TotalCost
        {
            get
            {
                if (RecipeIngredients != null && RecipeIngredients.Any())
                {
                    // Calcula a soma dos custos dos ingredientes apenas se nenhum deles for null
                    return RecipeIngredients.Where(ri => ri != null && ri.Ingredient != null)
                                            .Sum(ri => ri.Ingredient.Cost);
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
