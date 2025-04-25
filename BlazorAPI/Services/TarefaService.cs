using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.DTOs.Usuario;
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
            TbTarefa tarefa = MapearParaUsuario(_idUsuario, _dadosTarefaCadastro);

            await iTarefaRepository.CadastrarTarefaAsync(tarefa);
        }

        public async Task AlterarTarefaAsync(TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = MapearParaUsuario(_dadosTarefaCadastro);

            await iTarefaRepository.AlterarTarefaAsync(tarefa);
        }

        private TbTarefa MapearParaUsuario(TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            TbTarefa tarefa = new TbTarefa
            {
                TaTitulo = _dadosTarefaCadastro.titulo,
                TaDescricao = _dadosTarefaCadastro.descricao,
                TaPrioridade = _dadosTarefaCadastro.prioridade,
                TaPrazo = _dadosTarefaCadastro.prazo,
                TaStatus = _dadosTarefaCadastro.status,
                TaData = DateTime.Now.ToString(),
            };

            return tarefa;
        }

        private TbTarefa MapearParaUsuario(int _idUsuario, TarefaCadastrarDTO _dadosTarefaCadastro)
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

        public async Task<List<TarefaConsultaDTO>> ListaTarefasIdAsync(int _idUsuario)
        {
            List<TbTarefa> listaTarefas = await iTarefaRepository.ListaTarefasIdAsync(_idUsuario);

            if (listaTarefas.Count == 0)
            {
                throw new KeyNotFoundException($"Não foi encontrado tarefas cadastrados.");
            }

            List<TarefaConsultaDTO> listaConsultaTarefa = MapearParaUsuario(listaTarefas);

            return listaConsultaTarefa;
        }

        private List<TarefaConsultaDTO> MapearParaUsuario(List<TbTarefa> _listaTarefas)
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
    }
}