using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Repository.Tarefa
{
    public interface ITarefaRepository
    {
        Task CadastrarTarefaAsync(TbTarefa _dadosTarefa);

        Task AlterarTarefaAsync(TbTarefa _dadosTarefa);

        Task DeletarTarefaAsync(int _idTarefa);

        Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario);

        Task<TbTarefa> BuscarTarefaAsync(int _idTarefa);

        Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario);

        Task<(List<TbTarefa> Items, int TotalCount)> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize);

        Task<(List<TbTarefa> Items, int TotalCount)> ListaTarefasPaginadasStatusAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status);

        Task<(int pendente, int emAndamento, int concluido)> BuscarQtdStatusTarefaAsync(int _idUsuario);

        Task<List<TbTarefa>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario);
    }
}