using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Service
{
    public interface ITarefaService
    {
        Task CadastrarTarefaAsync(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro);

        Task AlterarTarefaAsync(TarefaCadastrarDTO _dadosTarefaCadastro);

        Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);
    }
}