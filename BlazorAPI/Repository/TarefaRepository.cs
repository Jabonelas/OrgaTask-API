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
            TbTarefa tarefa = await context.TbTarefas.FirstOrDefaultAsync(x => x.IdTarefa == _dadosTarefa.IdTarefa);

            tarefa.TaTitulo = _dadosTarefa.TaTitulo;
            tarefa.TaDescricao = _dadosTarefa.TaDescricao;
            tarefa.TaPrazo = _dadosTarefa.TaPrazo;
            tarefa.TaPrioridade = _dadosTarefa.TaPrioridade;
            tarefa.TaStatus = _dadosTarefa.TaStatus;

            await context.SaveChangesAsync();
        }

        public async Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario)
        {
            return await context.TbTarefas.AnyAsync(x => x.IdTarefa == _idTarefa && x.FkUsuario == _idUsuario);
        }

        public async Task DeletarTarefaAsync(int _idTarefa)
        {
            TbTarefa tarefa = await BuscarTarefaAsync(_idTarefa);

            if (tarefa != null)
            {
                context.TbTarefas.Remove(tarefa);
                await context.SaveChangesAsync();
            }
        }

        public async Task<TbTarefa> BuscarTarefaAsync(int _idTarefa)
        {
            return await context.TbTarefas.FirstOrDefaultAsync(x => x.IdTarefa == _idTarefa);
        }

        public async Task<(List<TbTarefa> Items, int TotalCount)> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize)
        {
            var query = context.TbTarefas
                .Where(x => x.FkUsuario == _idUsuario)
                .OrderByDescending(x => x.IdTarefa);

            var qtdTarefas = query.CountAsync();

            var tarefasPaginadas = query
                .Skip((_pageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            // Aguarda ambas as tarefas completarem
            await Task.WhenAll(qtdTarefas, tarefasPaginadas);

            // Retorna os itens e o total
            return (tarefasPaginadas.Result, qtdTarefas.Result);
        }

        public async Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefa = await context.TbTarefas
     .Where(x => x.FkUsuario == _idUsuario)
     .OrderByDescending(x => x.IdTarefa)
     .ToListAsync();

            return listaTarefa;
        }
    }
}