using BlazorAPI.DTOs.Tarefa;

namespace BlazorAPI.Interfaces.Service.Cache
{
    public interface ITarefaCacheService
    {
        Task<List<TarefaConsultaDTO>> GetListaTarefasCacheAsync(int _idUsuario);

        Task AtualziarTarefasCacheAsync(int _idUsuario, List<TarefaConsultaDTO> _tarefas);

        Task InvalidarCache(int _idUsuario);
    }
}