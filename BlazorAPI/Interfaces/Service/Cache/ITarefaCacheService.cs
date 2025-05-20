using BlazorAPI.DTOs.Tarefa;

namespace BlazorAPI.Interfaces.Service.Cache
{
    public interface ITarefaCacheService
    {
        Task<List<TarefaConsultaDTO>> GetTarefasCacheAsync(int usuarioId);

        Task AtualziarTarefasCacheAsync(int usuarioId, List<TarefaConsultaDTO> tarefas);

        Task InvalidateCacheAsync(int usuarioId);
    }
}