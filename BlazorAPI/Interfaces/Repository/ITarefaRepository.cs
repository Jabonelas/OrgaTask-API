using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Repository
{
    public interface ITarefaRepository
    {
        Task CadastrarTarefaAsync(TbTarefa _dadosTarefa);

        Task AlterarTarefaAsync(TbTarefa _dadosTarefa);

        Task DeletarTarefaAsync(int _idTarefa);

        Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario);

        Task<TbTarefa> BuscarTarefaAsync(int _idTarefa);

        Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario);
    }
}