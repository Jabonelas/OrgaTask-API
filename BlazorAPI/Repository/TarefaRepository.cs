using BlazorAPI.Interfaces.Repository.Tarefa;
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
            await context.TbTarefas.AddAsync(_dadosTarefa);
        }

        public async Task AlterarTarefaAsync(TbTarefa _dadosTarefa)
        {
            TbTarefa tarefa = await context.TbTarefas.FirstOrDefaultAsync(x => x.IdTarefa == _dadosTarefa.IdTarefa);

            tarefa.TaTitulo = _dadosTarefa.TaTitulo;
            tarefa.TaDescricao = _dadosTarefa.TaDescricao;
            tarefa.TaPrazo = _dadosTarefa.TaPrazo;
            tarefa.TaPrioridade = _dadosTarefa.TaPrioridade;
            tarefa.TaStatus = _dadosTarefa.TaStatus;
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

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((_pageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<TbTarefa> Items, int TotalCount)> ListaTarefasPaginadasStatusAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status)
        {
            var query = context.TbTarefas
                     .Where(x => x.FkUsuario == _idUsuario && x.TaStatus == _status)
                     .OrderByDescending(x => x.IdTarefa);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((_pageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<TbTarefa>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefa = await context.TbTarefas
     .Where(x => x.FkUsuario == _idUsuario)
     .OrderByDescending(x => x.IdTarefa)
     .ToListAsync();

            return listaTarefa;
        }

        public async Task<(int pendente, int emAndamento, int concluido)> BuscarQtdStatusTarefaAsync(int _idUsuario)
        {
            var tarefas = await context.TbTarefas
              .Where(x => (x.TaStatus == "Pendente" ||
                          x.TaStatus == "Em Progresso" ||
                          x.TaStatus == "Concluído") &&
                          x.FkUsuario == _idUsuario)
              .GroupBy(x => x.TaStatus)
              .Select(g => new
              {
                  Status = g.Key,
                  Count = g.Count()
              })
              .ToListAsync();

            return (
                pendente: tarefas.FirstOrDefault(x => x.Status == "Pendente")?.Count ?? 0,
                emProgresso: tarefas.FirstOrDefault(x => x.Status == "Em Progresso")?.Count ?? 0,
                concluido: tarefas.FirstOrDefault(x => x.Status == "Concluído")?.Count ?? 0
            );
        }

        public async Task<List<TbTarefa>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefasPrioridadeAlta = await context.TbTarefas.Where(x => x.TaPrioridade == "Alta" && x.TaStatus != "Concluído" && x.FkUsuario == _idUsuario).OrderBy(x => x.IdTarefa).ToListAsync();

            return listaTarefasPrioridadeAlta;
        }
    }
}