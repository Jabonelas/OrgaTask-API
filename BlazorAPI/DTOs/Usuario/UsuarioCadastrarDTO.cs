using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.DTOs.Usuario
{
    public class UsuarioCadastrarDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório!")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string nome { get; set; }

        [Required(ErrorMessage = "O login é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O login deve ter no máximo 50 caracteres.")]
        public string login { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O senha deve ter no máximo 50 caracteres.")]
        public string senha { get; set; }
    }
}