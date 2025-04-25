using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Repository
{
    public interface ITarefaRepository
    {
        Task CadastrarTarefaAsync(TbTarefa _dadosTarefa);

        Task AlterarTarefaAsync(TbTarefa _dadosTarefa);

        Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario);
    }
}