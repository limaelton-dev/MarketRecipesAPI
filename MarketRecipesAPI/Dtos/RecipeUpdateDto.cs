using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Dtos
{
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None, Id = "RecipeUpdateDto")]
    public class RecipeUpdateDto
    {
        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }
        public string Description { get; set; } // Descrição da receita
        public List<UpdateRecipeIngredientDto> Ingredients { get; set; }

        [JsonObject(ItemTypeNameHandling = TypeNameHandling.None, Id = "UpdateRecipeIngredientDto")]
        public class UpdateRecipeIngredientDto
        {
            [Required]
            public int IngredientId { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
            public float Quantity { get; set; } // Quantidade de cada produto na receita
        }
    }
}
