﻿using System.Runtime.InteropServices;
using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;

namespace BlazorAPI.Services.Tarefa
{
    public class TarefaService : ITarefaService
    {
        private readonly IUnitOfWork iUnitOfWork;

        public TarefaService(IUnitOfWork _unitOfWork, IAutenticacao _iAutenticacao)
        {
            iUnitOfWork = _unitOfWork;
        }

        public async Task CadastrarTarefaAsync(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro, string _prioridade, string _status)
        {
            TbTarefa tarefa = _dadosTarefaCadastro;
            tarefa.TaPrioridade = _prioridade;
            tarefa.TaStatus = _status.Replace("_", " ");
            tarefa.FkUsuario = _idUsuario;

            DateTimeOffset dataAtualBrasil = DateTimeOffset.UtcNow.ToOffset(new TimeSpan(-3, 0, 0)); 
            tarefa.TaData = dataAtualBrasil.ToString("yyyy-MM-dd HH:mm:ss");


            await iUnitOfWork.TarefaRepository.CadastrarTarefaAsync(tarefa);
            await iUnitOfWork.SalvarBancoAsync();
        }

        public async Task DeletarTarefaAsync(int _idTarefa, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario))
            {
                throw new InvalidOperationException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            await iUnitOfWork.TarefaRepository.DeletarTarefaAsync(_idTarefa);
            await iUnitOfWork.SalvarBancoAsync();
        }

        public async Task AlterarTarefaAsync(int _idUsuario, TarefaAlterarDTO _dadosTarefaCadastro, string _prioridade, string _status)
        {
            if (!await TarefaPertenceUsuarioAsync(_dadosTarefaCadastro.Id, _idUsuario))
            {
                throw new InvalidOperationException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = _dadosTarefaCadastro;
            tarefa.TaPrioridade = _prioridade;
            tarefa.TaStatus = _status.Replace("_", " ");

            await iUnitOfWork.TarefaRepository.AlterarTarefaAsync(tarefa);
            await iUnitOfWork.SalvarBancoAsync();
        }

        private async Task<bool> TarefaPertenceUsuarioAsync(int _idTarefa, int _idUsuario)
        {
            return await iUnitOfWork.TarefaRepository.TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario);
        }

        public async Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefas = await iUnitOfWork.TarefaRepository.ListaTarefasIdAsync(_idUsuario);

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

            if (string.IsNullOrEmpty(_status) || _status.ToLower() == "todas")
            {
                listaTarefa = await iUnitOfWork.TarefaRepository.ListaTarefasPaginadasAsync(_idUsuario, _pageNumber, _pageSize);
            }
            else
            {
                listaTarefa = await iUnitOfWork.TarefaRepository.ListaTarefasPaginadasStatusAsync(_idUsuario, _pageNumber, _pageSize, _status.Replace("_", " "));
            }

            return MapearTarefaPaginacao(listaTarefa);
        }

        public async Task<TarefaConsultaDTO> BuscarTarefaAsync(int _idTarefa, int _idUsuario)
        {
            if (!await TarefaPertenceUsuarioAsync(_idTarefa, _idUsuario))
            {
                throw new UnauthorizedAccessException("Operação não autorizada: a tarefa não pertence ao usuário atual.");
            }

            TbTarefa tarefa = await iUnitOfWork.TarefaRepository.BuscarTarefaAsync(_idTarefa);

            if (tarefa == null)
            {
                throw new KeyNotFoundException("Tarefa não encontrada");
            }

            TarefaConsultaDTO tarefaConsultaDTO = tarefa;

            return tarefaConsultaDTO;
        }

        public async Task<TarefaQtdStatusDTO> BuscarQtdStatusEPorcentagemConclusaoAsync(int _idUsuario)
        {
            var (pendente, emAndamento, concluido) = await iUnitOfWork.TarefaRepository.BuscarQtdStatusTarefaAsync(_idUsuario);

            var porcentagem = CalcularPorcentagemConclusaoTarefas(pendente, emAndamento, concluido);

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
            List<TbTarefa> listaTarefas = await iUnitOfWork.TarefaRepository.BuscarTarefasPrioridadeAltaAsync(_idUsuario);

            List<TarefaPrioridadeAltaDTO> listaPrioridadeAlta = new List<TarefaPrioridadeAltaDTO>();

            foreach (var item in listaTarefas)
            {
                DateTime hoje = DateTime.Today;
                DateTime dataTarefa = Convert.ToDateTime(item.TaData).Date;
                TimeSpan diferenca = hoje.Date - dataTarefa;
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
                    DataCriacao = t.TaData

                }).ToList(),

                TotalCount = _listaTarefa.TotalCount,
            };

            return result;
        }

        #endregion Mapear Tarefas

        #region Metodo Privado

        private decimal CalcularPorcentagemConclusaoTarefas(int _pendente, int _emAndamento, int _concluido)
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