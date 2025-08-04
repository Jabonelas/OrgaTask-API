using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Models;
using BlazorAPI.Repository.Tarefa;
using BlazorAPI.Repository.Usuario;
using Microsoft.EntityFrameworkCore;
using OrgaTask_API.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Repository
{
    public class TarefaRepositoryTest
    {

        private BlazorAPIBancodbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<BlazorAPIBancodbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new BlazorAPIBancodbContext(options);
        }

        #region Cadastrar Tarefa

        [Fact(DisplayName = "Cadastrar Tarefa: Deve salvar corretamente uma nova tarefa")]
        public async Task CadastrarTarefaQuandoDadosValidosDeveSalvarTarefaNoBanco()
        {
            // Arrange
            var contexto = CriarContextoInMemory();
            var repository = new TarefaRepository(contexto);

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosBuscarTarefa();

            // Act
            await repository.CadastrarTarefaAsync(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            // Assert
            var tarefaSalva = await contexto.TbTarefas.FirstOrDefaultAsync(t => t.IdTarefa == dadosTarefaMock.IdTarefa);

            Assert.NotNull(tarefaSalva);
            Assert.Equal(dadosTarefaMock.TaTitulo, tarefaSalva.TaTitulo);
        }

        #endregion


        #region Alterar Tarefa


        [Fact(DisplayName = "Alterar Tarefa: Deve atualizar todos os campos da tarefa corretamente")]
        public async Task AlterarTarefaQuandoTarefaExisteDeveAtualizarTodosCampos()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosTarefa();
            contexto.TbTarefas.Add(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            TbTarefa dadosTarefaAlterarMock = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            var repository = new TarefaRepository(contexto);

            // Act
            await repository.AlterarTarefaAsync(dadosTarefaAlterarMock);
            await contexto.SaveChangesAsync();

            // Assert
            var retorno = await contexto.TbTarefas.AnyAsync(u => u.IdTarefa == dadosTarefaMock.IdTarefa && u.TaTitulo == dadosTarefaAlterarMock.TaTitulo);

            Assert.True(retorno);
        }

        #endregion


        #region Tarefa Pertence ao Usuario


        [Fact(DisplayName = "Tarefa Pertence Usuario: Deve retornar true quando tarefa pertence ao usuário")]
        public async Task TarefaPertenceUsuarioTarefaDoUsuarioRetornaTrue()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosTarefa();
            contexto.TbTarefas.Add(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            bool retorno = await repository.TarefaPertenceUsuarioAsync(dadosTarefaMock.IdTarefa, dadosTarefaMock.FkUsuario);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.True(retorno);
        }


        [Fact(DisplayName = "Tarefa Pertence Usuario: Deve retornar false quando tarefa não pertence ao usuário")]
        public async Task TarefaPertenceUsuarioTarefaNaoDoUsuarioRetornaFalse()
        {
            // Arrange
            int idUsuario = 2;
            var contexto = CriarContextoInMemory();

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosTarefa();
            contexto.TbTarefas.Add(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            bool retorno = await repository.TarefaPertenceUsuarioAsync(dadosTarefaMock.IdTarefa, idUsuario);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.False(retorno);
        }

        #endregion


        #region Deletar Tarefa


        [Fact(DisplayName = "Deletar Tarefa: Deve remover a tarefa do banco quando tarefa existe")]
        public async Task DeletarTareTarefaExistenteDeveRemoverTarefa()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosTarefa();
            contexto.TbTarefas.Add(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            await repository.DeletarTarefaAsync(dadosTarefaMock.IdTarefa);
            await contexto.SaveChangesAsync();

            // Assert
            var retorno = await contexto.TbTarefas.AnyAsync(u => u.IdTarefa == dadosTarefaMock.IdTarefa);

            Assert.False(retorno);
        }

        #endregion


        #region Buscar Tarefa

        [Fact(DisplayName = "Buscar Tarefa: Deve retornar a tarefa correspondente quando tarefa existe")]
        public async Task BuscarTarefaTarefaExistenteDeveRetornarTarefa()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosTarefa();
            contexto.TbTarefas.Add(dadosTarefaMock);
            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            TbTarefa retorno = await repository.BuscarTarefaAsync(dadosTarefaMock.IdTarefa);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.NotNull(retorno);
        }

        #endregion


        #region Lista Tarefas Paginadas

        [Fact(DisplayName = "Lista Tarefas Paginada: Deve retornar lista paginada quando tarefas existem")]
        public async Task ListaTarefasPaginadasQuandoTarefasExistemDeveRetornarListaPaginada()
        {
            // Arrange
            int idUsuario = 1;
            int pageNumber = 1;
            int pageSize = 12;

            var contexto = CriarContextoInMemory();

            List<TbTarefa> listaTarefaMock = TarefaTestDataFactory.CriarDadosListaTarefa();

            listaTarefaMock.ForEach(t => contexto.TbTarefas.Add(t));

            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);
            
            // Act
            (List<TbTarefa> Items, int TotalCount) retorno = await repository.ListaTarefasPaginadasAsync(idUsuario,pageNumber,pageSize);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.NotNull(retorno);
        }

        #endregion


        #region Lista Tarefas Paginadas por Status

        [Fact(DisplayName = "Lista Tarefas Paginada Por Estatus: Deve retornar lista paginada quando tarefas existem")]
        public async Task ListaTarefasPaginadasPorStatusQuandoTarefasExistemDeveRetornarListaPaginada()
        {
            // Arrange
            int idUsuario = 1;
            int pageNumber = 1;
            int pageSize = 12;
            string status = "Pendente";

            var contexto = CriarContextoInMemory();

            List<TbTarefa> listaTarefaMock = TarefaTestDataFactory.CriarDadosListaTarefa();

            listaTarefaMock.ForEach(t => contexto.TbTarefas.Add(t));

            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            (List<TbTarefa> Items, int TotalCount) retorno = await repository.ListaTarefasPaginadasStatusAsync(idUsuario, pageNumber, pageSize, status);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.NotNull(retorno);
        }

        #endregion



        #region Lista Tarefas por IdUsuario

        [Fact(DisplayName = "Lista Tarefas IdUsuario: Deve retornar lista correspondente quando tarefas existem para o usuário")]
        public async Task ListaTarefasIdQuandoTarefasExistemParaUsuarioDeveRetornarListaCorrespondente()
        {
            // Arrange
            int idUsuario = 1;
         
            var contexto = CriarContextoInMemory();

            List<TbTarefa> listaTarefaMock = TarefaTestDataFactory.CriarDadosListaTarefa();

            listaTarefaMock.ForEach(t => contexto.TbTarefas.Add(t));

            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            List<TbTarefa> retorno = await repository.ListaTarefasIdAsync(idUsuario);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.NotNull(retorno);
        }

        #endregion


        #region Buscar Qtd Status Tarefa

        [Fact(DisplayName = "Buscar Qtd Status Tarefa: Deve retornar contagem correta por status quando tarefas existem")]
        public async Task BuscarQtdStatusTarefaComTarefasExistentesDeveRetornarContagemCorretaPorStatus()
        {
            // Arrange
            int idUsuario = 1;

            var contexto = CriarContextoInMemory();

            List<TbTarefa> listaTarefaMock = TarefaTestDataFactory.CriarDadosListaTarefa();

            listaTarefaMock.ForEach(t => contexto.TbTarefas.Add(t));

            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            (int pendente, int emAndamento, int concluido) retorno = await repository.BuscarQtdStatusTarefaAsync(idUsuario);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.Equal(2, retorno.pendente);       
            Assert.Equal(0, retorno.emAndamento);    
            Assert.Equal(0, retorno.concluido);      
        }

        #endregion


        #region Buscar Qtd Status Tarefa

        [Fact(DisplayName = "Buscar Tarefas Prioridade Alta: Deve retornar lista de tarefas com prioridade alta quando tarefas existem")]
        public async Task BuscarTarefasPrioridadeAltaQuandoTarefasExistemDeveRetornarListaPrioridadeAlta()
        {
            // Arrange
            int idUsuario = 1;

            var contexto = CriarContextoInMemory();

            List<TbTarefa> listaTarefaMock = TarefaTestDataFactory.CriarDadosListaTarefa();

            listaTarefaMock.ForEach(t => contexto.TbTarefas.Add(t));

            await contexto.SaveChangesAsync();

            var repository = new TarefaRepository(contexto);

            // Act
            List<TbTarefa> retorno = await repository.BuscarTarefasPrioridadeAltaAsync(idUsuario);
            await contexto.SaveChangesAsync();

            // Assert
            Assert.NotNull(retorno);
        }

        #endregion
    }
}
