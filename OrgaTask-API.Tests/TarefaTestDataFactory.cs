using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;

namespace OrgaTask_API.Tests
{
    public static class TarefaTestDataFactory
    {
        public static TarefaCadastrarDTO CriarDadosCadastrarTarefa()
        {
            return new TarefaCadastrarDTO()
            {
                Titulo = "Titulo Teste",
                Descricao = "Descricao Teste",
                Prazo = 1,
            };
        }

        public static TarefaAlterarDTO CriarDadosAlterarTarefa()
        {
            return new TarefaAlterarDTO
            {
                Id = 1,
                Titulo = "Titulo Teste",
                Descricao = "Descricao Teste",
                Prazo = 1,
            };
        }


        public static PagedResult<TarefaConsultaDTO> CriarDadosListaTarefaPaginada()
        {
            PagedResult<TarefaConsultaDTO> listaTarefaPagianda = new PagedResult<TarefaConsultaDTO>
            {
                TotalCount = 2,
                Items = new List<TarefaConsultaDTO>
                {
                    new TarefaConsultaDTO
                    {
                        Id = 1,
                        Titulo = "Tarefa 1" ,
                        Descricao = "Descricao 1" ,
                        Prioridade = "Alta" ,
                        Prazo = 1 ,
                        Status = "Pendente" ,
                        DataCriacao = "Data Criacao 1",

                    },
                    new TarefaConsultaDTO
                    {
                        Id = 2,
                        Titulo = "Tarefa 2" ,
                        Descricao = "Descricao 2" ,
                        Prioridade = "Alta" ,
                        Prazo = 2 ,
                        Status = "Pendente" ,
                        DataCriacao = "Data Criacao 2",

                    },
                }
            };

            return listaTarefaPagianda;
        }


        public static TarefaConsultaDTO CriarDadosBuscarTarefa()
        {
            return new TarefaConsultaDTO()
            {
                Id = 1,
                Titulo = "Tarefa 1",
                Descricao = "Descricao 1",
                Prioridade = "Alta",
                Prazo = 1,
                Status = "Pendente",
                DataCriacao = "Data Criacao 1",
            };
        }



        public static TarefaQtdStatusDTO CriarDadosBuscarQtdStatusEPorcentagemConclusao()
        {
            return new TarefaQtdStatusDTO()
            {
                Pendente = 1,
                EmProgresso = 2,
                Concluido = 3,
                PorcentagemConcluidas = 4,
            };
        }


        public static List<TarefaPrioridadeAltaDTO> CriarDadosBuscarTarefasPrioridadeAlta()
        {
            List<TarefaPrioridadeAltaDTO> listaTarefaPrioridadeAlta = new List<TarefaPrioridadeAltaDTO>
            {
                    new TarefaPrioridadeAltaDTO
                    {
                        Id = 1,
                        Titulo = "Tarefa 1" ,
                        Prazo = 1 ,
                        Status = "Pendente" ,

                    },
                    new TarefaPrioridadeAltaDTO
                    {
                        Id = 2,
                        Titulo = "Tarefa 2" ,
                        Prazo = 2 ,
                        Status = "Pendente" ,

                    },
            };

            return listaTarefaPrioridadeAlta;
        }
    }
}
