using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Dtos
{
    public class IngredientUpdateDto
    {
        [Required(ErrorMessage = "O nome do ingrediente é obrigatório.")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O custo do ingrediente deve ser maior que zero.")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        public string Unit { get; set; }
    }
}
