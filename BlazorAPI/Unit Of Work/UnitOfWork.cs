using BlazorAPI.Interfaces.Repository.Tarefa;
using BlazorAPI.Interfaces.Repository.Usuario;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;
using BlazorAPI.Repository;

namespace BlazorAPI.Unit_O_fWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlazorAPIBancodbContext context;
        public IUsuarioRepository UsuarioReposity { get; }
        public ITarefaRepository TarefaRepository { get; }

        public UnitOfWork(BlazorAPIBancodbContext _context)
        {
            context = _context;
            UsuarioReposity = new UsuarioRepository(_context);
            TarefaRepository = new TarefaRepository(_context);
        }

        public async Task<int> SalvarBancoAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}