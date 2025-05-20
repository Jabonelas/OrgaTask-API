using Blazor_WebAssembly.DTOs.Tarefa;
using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;

namespace BlazorAPI.Interfaces.Service.Tarefa;

public interface ITarefaService
{
    Task CadastrarTarefaAsync(int idUsuario, TarefaCadastrarDTO dadosTarefaCadastro);

    Task AlterarTarefaAsync(TarefaCadastrarDTO dadosTarefaCadastro, int idUsuario);

    Task DeletarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);

    Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize);

    Task<TarefaCadastrarDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<TarefaQtdStatus> BuscarQtdStatusTarefaAsync(int _idUsuario);
}