using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Models;

namespace BlazorAPI.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository iTarefaRepository;
        private readonly IAutenticacao iAutenticacao;

        public TarefaService(ITarefaRepository _iTarefaRepository, IAutenticacao _iAutenticacao)
        {
            iTarefaRepository = _iTarefaRepository;

            iAutenticacao = _iAutenticacao;
        }

        public async Task CadastrarTarefaAsync(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = _dadosTarefaCadastro;

            tarefa.FkUsuario = _idUsuario;

            await iTarefaRepository.CadastrarTarefaAsync(tarefa);
        }

        public async Task DeletarTarefaAsync(int _idTarefa, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            await iTarefaRepository.DeletarTarefaAsync(_idTarefa);
        }

        public async Task AlterarTarefaAsync(TarefaCadastrarDTO _dadosTarefaCadastro, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_dadosTarefaCadastro.Id, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = _dadosTarefaCadastro;

            await iTarefaRepository.AlterarTarefaAsync(tarefa);
        }

        private async Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario)
        {
            return await iTarefaRepository.TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario);
        }

        #region Mapear Tarefas

        private PagedResult<TarefaConsultaDTO> MapearTarefaPaginacao((List<TbTarefa> Items, int TotalCount) _listaTarefa)
        {
            var result = new PagedResult<TarefaConsultaDTO>
            {
                Items = _listaTarefa.Items.Select(t => new TarefaConsultaDTO
                {
                    Id = t.IdTarefa,
                    Titulo = t.TaTitulo,
                    Descricao = t.TaDescricao,
                    Prioridade = t.TaPrioridade,
                    Prazo = t.TaPrazo,
                    Status = t.TaStatus,
                    Data = t.TaData,
                }).ToList(),

                TotalCount = _listaTarefa.TotalCount,
            };

            return result;
        }

        #endregion Mapear Tarefas

        public async Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefas = await iTarefaRepository.ListaTarefasIdAsync(_idUsuario);

            if (listaTarefas.Count == 0)
            {
                throw new KeyNotFoundException($"Nenhuma tarefa encontrada para o usuário ID {_idUsuario}");
            }

            List<TarefaConsultaDTO> listaConsultaTarefa = listaTarefas.Select(t => (TarefaConsultaDTO)t).ToList();

            return listaConsultaTarefa;
        }

        public async Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize)
        {
            (List<TbTarefa> Items, int TotalCount) listaTarefa = await iTarefaRepository.ListaTarefasPaginadasAsync(_idUsuario, _pageNumber, _pageSize);

            if (listaTarefa.TotalCount == 0)
            {
                throw new KeyNotFoundException($"Nenhuma tarefa encontrada para o usuário ID {_idUsuario}");
            }

            return MapearTarefaPaginacao(listaTarefa);
        }

        public async Task<TarefaCadastrarDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = await iTarefaRepository.BuscarTarefaAsync(_idTarefa);

            TarefaCadastrarDTO tarefaCadastrarDTO = tarefa;

            return tarefaCadastrarDTO;
        }
    }
}