using BlazorAPI.Models;
using BlazorAPI.Repository.Usuario;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorAPI.Repository.Tarefa;
using OrgaTask_API.Tests;
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
            var retorno = await contexto.TbTarefas.FirstOrDefaultAsync(u => u.IdTarefa == dadosTarefaMock.IdTarefa);

            Assert.NotNull(retorno);
        }

        #endregion


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
    }
}
