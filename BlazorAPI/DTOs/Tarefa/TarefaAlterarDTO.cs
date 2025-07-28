using System.ComponentModel.DataAnnotations;
using BlazorAPI.Models;

namespace BlazorAPI.DTOs.Tarefa
{
    public class TarefaAlterarDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório!")]
        [MaxLength(50, ErrorMessage = "O título deve ter no máximo 50 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório!")]
        [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O prazo é obrigatório!")]
        [Range(1, 999)]
        public int Prazo { get; set; }


        public static implicit operator TarefaAlterarDTO(TbTarefa _tarefa) =>
            new()
            {
                Id = _tarefa.IdTarefa,
                Titulo = _tarefa.TaTitulo,
                Descricao = _tarefa.TaDescricao,
            };
    }
}