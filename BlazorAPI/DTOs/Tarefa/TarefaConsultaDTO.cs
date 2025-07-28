using BlazorAPI.Models;

namespace BlazorAPI.DTOs.Tarefa
{
    public class TarefaConsultaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Prioridade { get; set; }
        public int Prazo { get; set; }
        public string Status { get; set; }
        public string DataCriacao { get; set; }


        public static implicit operator TarefaConsultaDTO(TbTarefa _tarefa) =>
            new()
            {
                Id = _tarefa.IdTarefa,
                Titulo = _tarefa.TaTitulo,
                Descricao = _tarefa.TaDescricao,
                Prioridade = _tarefa.TaPrioridade,
                Prazo = _tarefa.TaPrazo,
                Status = _tarefa.TaStatus,
                DataCriacao = _tarefa.TaData

            };
    }
}