using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Dtos
{
    public class RecipeCreateDto
    {
        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Os IDs dos ingredientes são obrigatórios.")]
        [MinLength(2, ErrorMessage = "A receita deve conter pelo menos 2 ingredientes.")]
        public List<int> IngredientIds { get; set; }
    }
}
