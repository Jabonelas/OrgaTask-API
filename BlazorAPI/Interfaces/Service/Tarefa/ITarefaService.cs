using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;

namespace BlazorAPI.Interfaces.Service.Tarefa;

public interface ITarefaService
{
    Task CadastrarTarefaAsync(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro, string _prioridade, string _status);

    Task AlterarTarefaAsync(int _idUsuario, TarefaAlterarDTO _dadosTarefaCadastro, string _prioridade, string _status);

    Task DeletarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario);

    Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status);

    Task<TarefaConsultaDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario);

    Task<TarefaQtdStatusDTO> BuscarQtdStatusEPorcentagemConclusaoAsync(int _idUsuario);

    Task<List<TarefaPrioridadeAltaDTO>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario);
}