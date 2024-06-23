using System.ComponentModel.DataAnnotations;

namespace MarketRecipesAPI.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome do ingrediente é obrigatório.")]
        public string Name { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "O custo do ingrediente deve ser maior que zero.")]
        public decimal Cost { get; set; }
        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        public string Unit { get; set; }
    }
}