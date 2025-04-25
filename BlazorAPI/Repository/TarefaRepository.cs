using BlazorAPI.Interfaces.Repository;
using BlazorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAPI.Repository
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly BlazorAPIBancodbContext context;

        public TarefaRepository(BlazorAPIBancodbContext _context)
        {
            context = _context;
        }

        public async Task CadastrarTarefaAsync(TbTarefa _dadosTarefa)
        {
            context.TbTarefas.Add(_dadosTarefa);
            await context.SaveChangesAsync();
        }

        public async Task AlterarTarefaAsync(TbTarefa _dadosTarefa)
        {
            context.TbTarefas.Update(_dadosTarefa);
            await context.SaveChangesAsync();
        }

        public async Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefa = await context.TbTarefas.Where(x => x.FkUsuario == _idUsuario).ToListAsync();

            return listaTarefa;
        }
    }
}