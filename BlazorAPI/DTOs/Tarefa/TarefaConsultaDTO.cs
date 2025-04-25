using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.DTOs.Tarefa
{
    public class TarefaConsultaDTO
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public string prioridade { get; set; }
        public int prazo { get; set; }
        public string status { get; set; }
        public string data { get; set; }
    }
}