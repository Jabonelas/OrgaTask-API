using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository.Tarefa;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Models;

namespace BlazorAPI.Services.Tarefa
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

        public async Task CadastrarTarefaAsync(int _idUsuario, TarefaDTO _dadosTarefaCadastro)
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

        public async Task AlterarTarefaAsync(TarefaDTO _dadosTarefaCadastro, int _idUsuario)
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

        public async Task<PagedResult<TarefaConsultaDTO>> ListaTarefasPaginadasAsync(int _idUsuario, int _pageNumber, int _pageSize, string _status)
        {
            (List<TbTarefa> Items, int TotalCount) listaTarefa;

            if (string.IsNullOrEmpty(_status) || _status == "todas")
            {
                listaTarefa = await iTarefaRepository.ListaTarefasPaginadasAsync(_idUsuario, _pageNumber, _pageSize);
            }
            else
            {
                listaTarefa = await iTarefaRepository.ListaTarefasPaginadasStatusAsync(_idUsuario, _pageNumber, _pageSize, _status);
            }

            return MapearTarefaPaginacao(listaTarefa);
        }

        public async Task<TarefaDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = await iTarefaRepository.BuscarTarefaAsync(_idTarefa);

            TarefaDTO tarefaCadastrarDTO = tarefa;

            return tarefaCadastrarDTO;
        }

        public async Task<TarefaQtdStatusDTO> ObterQtdStatusEPorcentagemConclusaoAsync(int _idUsuario)
        {
            var (pendente, emAndamento, concluido) = await iTarefaRepository.BuscarQtdStatusTarefaAsync(_idUsuario);

            var porcentagem = CalcularPorcentagemConclusao(pendente, emAndamento, concluido);

            TarefaQtdStatusDTO tarefaQtdStatus = new TarefaQtdStatusDTO()
            {
                Pendente = pendente,
                EmProgresso = emAndamento,
                Concluido = concluido,
                PorcentagemConcluidas = porcentagem,
            };

            return tarefaQtdStatus;
        }

        public async Task<List<TarefaPrioridadeAltaDTO>> BuscarTarefasPrioridadeAltaAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefas = await iTarefaRepository.BuscarTarefasPrioridadeAltaAsync(_idUsuario);

            List<TarefaPrioridadeAltaDTO> listaPrioridadeAlta = new List<TarefaPrioridadeAltaDTO>();

            foreach (var item in listaTarefas)
            {
                DateTime hoje = DateTime.Today;

                DateTime.TryParse(item.TaData?.ToString(), out DateTime dataTarefa);
                TimeSpan diferenca = hoje - dataTarefa;
                int diasDiferenca = diferenca.Days;

                int diasDiferente = -diasDiferenca + item.TaPrazo;

                listaPrioridadeAlta.Add(new TarefaPrioridadeAltaDTO
                {
                    Id = item.IdTarefa,
                    Titulo = item.TaTitulo,
                    Data = item.TaData,
                    Status = item.TaStatus,
                    Prazo = diasDiferente
                });
            }

            listaPrioridadeAlta.OrderByDescending(x => x.Status);

            return listaPrioridadeAlta;
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

        #region Metodo Privado

        private decimal CalcularPorcentagemConclusao(int _pendente, int _emAndamento, int _concluido)
        {
            decimal soma = _pendente + _emAndamento + _concluido;
            decimal porcentagem = 0;

            if (soma != 0)
            {
                decimal porcentagemTarefasConcluidas = _concluido * 100m / soma;

                porcentagem = Math.Round(porcentagemTarefasConcluidas, 2);
            }

            return porcentagem;
        }

        #endregion Metodo Privado
    }
}