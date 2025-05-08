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
            if (!await TarefaPertenceUsuarioAsync(_dadosTarefaCadastro.Id, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            //TbTarefa tarefa = MapearParaTarefa(_dadosTarefaCadastro);

            TbTarefa tarefa = _dadosTarefaCadastro;

            await iTarefaRepository.AlterarTarefaAsync(tarefa);
        }

        private async Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario)
        {
            return await iTarefaRepository.TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario);
        }

        #region Mapear Tarefas

        //private TbTarefa MapearParaTarefa(TarefaCadastrarDTO _dadosTarefaCadastro)
        //{
        //    TbTarefa tarefa = new TbTarefa
        //    {
        //        IdTarefa = _dadosTarefaCadastro.Id,
        //        TaTitulo = _dadosTarefaCadastro.Titulo,
        //        TaDescricao = _dadosTarefaCadastro.Descricao,
        //        TaPrioridade = _dadosTarefaCadastro.Prioridade,
        //        TaPrazo = _dadosTarefaCadastro.Prazo,
        //        TaStatus = _dadosTarefaCadastro.Status,
        //        TaData = DateTime.Now.ToString(),
        //    };

        //    return tarefa;
        //}

        private TbTarefa MapearParaTarefaFK(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = new TbTarefa
            {
                TaTitulo = _dadosTarefaCadastro.Titulo,
                TaDescricao = _dadosTarefaCadastro.Descricao,
                TaPrioridade = _dadosTarefaCadastro.Prioridade,
                TaPrazo = _dadosTarefaCadastro.Prazo,
                TaStatus = _dadosTarefaCadastro.Status,
                TaData = DateTime.Now.ToString(),
                FkUsuario = _idUsuario
            };

            return tarefa;
        }

        //private List<TarefaConsultaDTO> MapearParaListaTarefas(List<TbTarefa> _listaTarefas)
        //{
        //    List<TarefaConsultaDTO> listaConsultaTarefa = new List<TarefaConsultaDTO>();

        //    foreach (var item in _listaTarefas)
        //    {
        //        listaConsultaTarefa.Add(new TarefaConsultaDTO
        //        {
        //            Id = item.IdTarefa,
        //            Titulo = item.TaTitulo,
        //            Descricao = item.TaDescricao,
        //            Prioridade = item.TaPrioridade,
        //            Prazo = item.TaPrazo,
        //            Status = item.TaStatus,
        //            Data = item.TaData
        //        });
        //    }

        //    return listaConsultaTarefa;
        //}

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

        //private TarefaCadastrarDTO MapearParaCadastroTarefaDTO(TbTarefa _dadosTarefaCadastro)
        //{
        //    if (_dadosTarefaCadastro == null)
        //    {
        //        return null;
        //    }

        //    TarefaCadastrarDTO tarefa = new TarefaCadastrarDTO
        //    {
        //        Id = _dadosTarefaCadastro.IdTarefa,
        //        Titulo = _dadosTarefaCadastro.TaTitulo,
        //        Descricao = _dadosTarefaCadastro.TaDescricao,
        //        Prioridade = _dadosTarefaCadastro.TaPrioridade,
        //        Prazo = _dadosTarefaCadastro.TaPrazo,
        //        Status = _dadosTarefaCadastro.TaStatus,
        //    };

        //    return tarefa;
        //}

        #endregion Mapear Tarefas

        public async Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario)
        {
            _idUsuario = 3;

            List<TbTarefa> listaTarefas = await iTarefaRepository.ListaTarefasIdAsync(_idUsuario);

            if (listaTarefas.Count == 0)
            {
                throw new KeyNotFoundException($"Nenhuma tarefa encontrada para o usuário ID {_idUsuario}");
            }

            //List<TarefaConsultaDTO> listaConsultaTarefa = MapearParaListaTarefas(listaTarefas);
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

            //TarefaCadastrarDTO tarefaCadastrarDTO = MapearParaCadastroTarefaDTO(tarefa);

            TarefaCadastrarDTO tarefaCadastrarDTO = tarefa;

            return tarefaCadastrarDTO;
        }
    }
}