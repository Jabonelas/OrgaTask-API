using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;
using System.Collections.Generic;

namespace OrgaTask_API.Tests
{
    public static class TarefaTestDataFactory
    {
        public static TarefaCadastrarDTO CriarDadosCadastrarTarefaDTO()
        {
            return new TarefaCadastrarDTO()
            {
                Titulo = "Titulo Teste",
                Descricao = "Descricao Teste",
                Prazo = 1,
            };
        }

        public static TarefaAlterarDTO CriarDadosAlterarTarefaDTO()
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


        public static (List<TbTarefa> Items, int TotalCount) CriarDadosListaTarefaPaginadaPorEstatus()
        {
            var listaTarefaPaginada = (
                Items: new List<TbTarefa>
                {
                    new TbTarefa
                    {
                        IdTarefa = 1,
                        TaTitulo = "Tarefa 1",
                        TaDescricao = "Descricao 1",
                        TaPrioridade = "Alta",
                        TaPrazo = 1,
                        TaStatus = "Pendente",
                        TaData = "Data Criacao 1",
                    },
                    new TbTarefa
                    {
                        IdTarefa = 2,
                        TaTitulo = "Tarefa 2",
                        TaDescricao = "Descricao 2",
                        TaPrioridade = "Alta",
                        TaPrazo = 2,
                        TaStatus = "Pendente",
                        TaData = "Data Criacao 2",
                    }
                },
                TotalCount: 2
            );

            return listaTarefaPaginada;
        }


        public static List<TbTarefa> CriarDadosListaTarefa()
        {
            List<TbTarefa> listaTarefa = new List<TbTarefa>
            {
                    new TbTarefa
                    {
                        IdTarefa = 1,
                        TaTitulo = "Tarefa 1" ,
                        TaDescricao = "Descricao 1" ,
                        TaPrioridade = "Alta" ,
                        TaPrazo = 1 ,
                        TaStatus = "Pendente" ,
                        TaData = DateTime.Now.ToString(),
                    },
                    new TbTarefa
                    {
                        IdTarefa = 2,
                        TaTitulo = "Tarefa 2" ,
                        TaDescricao =  "Descricao 2" ,
                        TaPrioridade = "Alta" ,
                        TaPrazo = 2 ,
                        TaStatus = "Pendente" ,
                        TaData = DateTime.Now.ToString(),
                    },
            };

            return listaTarefa;
        }

        public static TarefaConsultaDTO CriarDadosBuscarTarefaDTO()
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
        
        public static TbTarefa CriarDadosBuscarTarefa()
        {
            return new TbTarefa
            {
                IdTarefa = 2,
                TaTitulo = "Tarefa 2",
                TaDescricao = "Descricao 2",
                TaPrioridade = "Alta",
                TaPrazo = 2,
                TaStatus = "Pendente",
                TaData = "Data Criacao 2",
            };
        }

        public static TbTarefa CriarDadosTarefa()
        {
            return new TbTarefa
            {
                IdTarefa = 3,
                TaTitulo = "Tarefa 2",
                TaDescricao = "Descricao 2",
                TaPrioridade = "Alta",
                TaPrazo = 2,
                TaStatus = "Pendente",
                TaData = "Data Criacao 2",
            };
        }

        public static TbTarefa CriarDadosAlterarTarefa()
        {
            return new TbTarefa
            {
                IdTarefa = 3,
                TaTitulo = "Tarefa Alterada 2",
                TaDescricao = "Descricao Alterada 2",
                TaPrioridade = "Alta",
                TaPrazo = 2,
                TaStatus = "Pendente",
                TaData = "Data Criacao 2",
            };
        }

        public static (int pendente, int emAndamento, int concluido) CriarDadosQtdStatus()
        {
            return (pendente: 0, emAndamento: 0, concluido: 0);
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
