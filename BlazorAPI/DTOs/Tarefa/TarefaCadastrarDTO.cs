using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.DTOs.Tarefa;

public class TarefaCadastrarDTO
{
    [Required(ErrorMessage = "O título é obrigatório!")]
    [MaxLength(50, ErrorMessage = "O título deve ter no máximo 50 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatório!")]
    [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O prazo é obrigatório!")]
    [Range(1, 999)]
    public int Prazo { get; set; }
}

