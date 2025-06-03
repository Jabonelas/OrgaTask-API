using BlazorAPI.Interfaces.Repository.Tarefa;
using BlazorAPI.Interfaces.Repository.Usuario;

namespace BlazorAPI.Interfaces.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        IUsuarioRepository UsuarioReposity { get; }
        ITarefaRepository TarefaRepository { get; }

        Task<int> SalvarBancoAsync();
    }
}