using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.DTOs.Tarefa
{
    public class TarefaCadastrarDTO
    {
        [Required(ErrorMessage = "O título é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O título deve ter no máximo 50 caracteres.")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório!")]
        [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string descricao { get; set; }

        [Required(ErrorMessage = "O prioridade é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O prioridade deve ter no máximo 50 caracteres.")]
        public string prioridade { get; set; }

        [Required(ErrorMessage = "O prazo é obrigatório!")]
        public int prazo { get; set; }

        [Required(ErrorMessage = "O status é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O status deve ter no máximo 50 caracteres.")]
        public string status { get; set; }
    }
}