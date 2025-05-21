using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Service.Tarefa;

public interface ITarefaService
{
    Task CadastrarTarefaAsync(int idUsuario, TarefaDTO dadosTarefaCadastro);

    Task AlterarTarefaAsync(TarefaDTO dadosTarefaCadastro, int idUsuario);

    Task DeletarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);

    Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status);

    Task<TarefaDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<TarefaQtdStatusDTO> BuscarQtdStatusTarefaAsync(int _idUsuario);

    Task<decimal> BuscarPorcentagemTarefaConcluidaAsync(int _idUsuario);

    Task<List<TarefaPrioridadeAltaDTO>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario);
}