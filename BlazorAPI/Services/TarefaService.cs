using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Models;
using BlazorAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            TbTarefa tarefa = MapearParaTarefaFK(_idUsuario, _dadosTarefaCadastro);

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
            if (!await TarefaPertenceUsuarioAsync(_dadosTarefaCadastro.id, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = MapearParaTarefa(_dadosTarefaCadastro);

            await iTarefaRepository.AlterarTarefaAsync(tarefa);
        }

        private async Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario)
        {
            return await iTarefaRepository.TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario);
        }

        #region Mapear Tarefas

        private TbTarefa MapearParaTarefa(TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = new TbTarefa
            {
                IdTarefa = _dadosTarefaCadastro.id,
                TaTitulo = _dadosTarefaCadastro.titulo,
                TaDescricao = _dadosTarefaCadastro.descricao,
                TaPrioridade = _dadosTarefaCadastro.prioridade,
                TaPrazo = _dadosTarefaCadastro.prazo,
                TaStatus = _dadosTarefaCadastro.status,
                TaData = DateTime.Now.ToString(),
            };

            return tarefa;
        }

        private TbTarefa MapearParaTarefaFK(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = new TbTarefa
            {
                TaTitulo = _dadosTarefaCadastro.titulo,
                TaDescricao = _dadosTarefaCadastro.descricao,
                TaPrioridade = _dadosTarefaCadastro.prioridade,
                TaPrazo = _dadosTarefaCadastro.prazo,
                TaStatus = _dadosTarefaCadastro.status,
                TaData = DateTime.Now.ToString(),
                FkUsuario = _idUsuario
            };

            return tarefa;
        }

        private List<TarefaConsultaDTO> MapearParaListaTarefas(List<TbTarefa> _listaTarefas)
        {
            List<TarefaConsultaDTO> listaConsultaTarefa = new List<TarefaConsultaDTO>();

            foreach (var item in _listaTarefas)
            {
                listaConsultaTarefa.Add(new TarefaConsultaDTO
                {
                    id = item.IdTarefa,
                    titulo = item.TaTitulo,
                    descricao = item.TaDescricao,
                    prioridade = item.TaPrioridade,
                    prazo = item.TaPrazo,
                    status = item.TaStatus,
                    data = item.TaData
                });
            }

            return listaConsultaTarefa;
        }

        private TarefaCadastrarDTO MapearParaCadastroTarefaDTO(TbTarefa _dadosTarefaCadastro)
        {
            if (_dadosTarefaCadastro == null)
            {
                return null;
            }

            TarefaCadastrarDTO tarefa = new TarefaCadastrarDTO
            {
                id = _dadosTarefaCadastro.IdTarefa,
                titulo = _dadosTarefaCadastro.TaTitulo,
                descricao = _dadosTarefaCadastro.TaDescricao,
                prioridade = _dadosTarefaCadastro.TaPrioridade,
                prazo = _dadosTarefaCadastro.TaPrazo,
                status = _dadosTarefaCadastro.TaStatus,
            };

            return tarefa;
        }

        private PagedResult<TarefaConsultaDTO> MapearTarefaPaginacao((List<TbTarefa> Items, int TotalCount) _listaTarefa)
        {
            var result = new PagedResult<TarefaConsultaDTO>
            {
                Items = _listaTarefa.Items.Select(t => new TarefaConsultaDTO
                {
                    id = t.IdTarefa,
                    titulo = t.TaTitulo,
                    descricao = t.TaDescricao,
                    prioridade = t.TaPrioridade,
                    prazo = t.TaPrazo,
                    status = t.TaStatus,
                    data = t.TaData,
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

            List<TarefaConsultaDTO> listaConsultaTarefa = MapearParaListaTarefas(listaTarefas);

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

            TarefaCadastrarDTO tarefaCadastrarDTO = MapearParaCadastroTarefaDTO(tarefa);

            return tarefaCadastrarDTO;
        }
    }
}