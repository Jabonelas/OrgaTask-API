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
    }
}