using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Dtos
{
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None, Id = "RecipeCreateDto")]
    public class RecipeCreateDto
    {
        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }

        public string Description { get; set; } // Descrição da receita

        [Required(ErrorMessage = "Os ingredientes são obrigatórios.")]
        [MinLength(2, ErrorMessage = "A receita deve conter pelo menos 2 ingredientes.")]
        public List<CreateRecipeIngredientDto> Ingredients { get; set; }

        [JsonObject(ItemTypeNameHandling = TypeNameHandling.None, Id = "CreateRecipeIngredientDto")]
        public class CreateRecipeIngredientDto
        {
            [Required]
            public int IngredientId { get; set; }

            [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
            public float Quantity { get; set; } // Quantidade de cada produto na receita
        }
    }
}
