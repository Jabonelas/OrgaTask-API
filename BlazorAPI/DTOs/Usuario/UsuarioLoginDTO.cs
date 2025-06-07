using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.DTOs.Usuario
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage = "O login é obrigatório!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "A senha é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O senha deve ter no máximo 50 caracteres.")]
        public string Senha { get; set; }
    }
}