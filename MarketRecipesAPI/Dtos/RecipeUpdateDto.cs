using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Dtos
{
    public class RecipeUpdateDto
    {
        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }
        public List<int> IngredientIds { get; set; }
    }

}
