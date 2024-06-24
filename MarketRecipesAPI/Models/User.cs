using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MarketRecipesAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Username { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }
    }
}
