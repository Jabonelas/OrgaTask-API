using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Service
{
    public interface ITarefaService
    {
        Task CadastrarTarefaAsync(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro);

        Task AlterarTarefaAsync(TarefaCadastrarDTO _dadosTarefaCadastro, int _idUsuario);

        Task DeletarTarefaAsync(int _idTarefa, int _idUsuario);

        Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);

        Task<TarefaCadastrarDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario);
    }
}