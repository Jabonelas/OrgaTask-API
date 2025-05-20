namespace Blazor_WebAssembly.DTOs.Tarefa
{
    public class TarefaQtdStatus
    {
        public int Pendente { get; set; }
        public int EmProgresso { get; set; }
        public int Concluido { get; set; }
    }

    public class TaskModel
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
    }
}