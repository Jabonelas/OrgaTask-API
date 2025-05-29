using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;

namespace BlazorAPI.Interfaces.Service.Tarefa;

public interface ITarefaService
{
    Task CadastrarTarefaAsync(int idUsuario, TarefaDTO dadosTarefaCadastro);

    Task AlterarTarefaAsync(TarefaDTO dadosTarefaCadastro, int idUsuario);

    Task DeletarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);

    Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status);

    Task<TarefaDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<TarefaQtdStatusDTO> ObterQtdStatusEPorcentagemConclusaoAsync(int _idUsuario);

    Task<List<TarefaPrioridadeAltaDTO>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario);
}