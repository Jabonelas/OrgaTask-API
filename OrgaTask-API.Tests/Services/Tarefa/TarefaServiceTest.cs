using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;
using BlazorAPI.Services.Tarefa;
using BlazorAPI.Services.Usuario;
using BlazorAPI.Unit_O_fWork;
using Moq;
using OrgaTask_API.Tests;
using System.Collections.Generic;
using Xunit;

namespace Services.Tarefa
{
    public class TarefaServiceTest
    {
        private readonly Mock<IUnitOfWork> iUnitOfWorkMock;

        private readonly Mock<IAutenticacao> iAutenticacaoMock;

        private readonly TarefaService tarefaService;

        public TarefaServiceTest()
        {
            iUnitOfWorkMock = new Mock<IUnitOfWork>();
            iAutenticacaoMock = new Mock<IAutenticacao>();
            tarefaService = new TarefaService(iUnitOfWorkMock.Object, iAutenticacaoMock.Object);
        }

        #region Cadastrar Tarefa

        [Fact(DisplayName = "Cadastrar Tarefa: Deve cadastrar tarefa no banco quando dados são válidos")]
        public async Task CadastrarTarefaDeveSalvarNoBancoQuandoDadosValidos()
        {
            // Arrange
            int idUsuario = 1;

            TarefaCadastrarDTO dadosTarefaMock = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.CadastrarTarefaAsync(It.IsAny<TbTarefa>())).Returns(Task.CompletedTask);

            iUnitOfWorkMock.Setup(u => u.SalvarBancoAsync());

            // Act
            await tarefaService.CadastrarTarefaAsync(idUsuario, dadosTarefaMock, "Alta", "Pendente");

            // Assert
            iUnitOfWorkMock.Verify(u => u.TarefaRepository.CadastrarTarefaAsync(It.IsAny<TbTarefa>()), Times.Once);

            iUnitOfWorkMock.Verify(u => u.SalvarBancoAsync(), Times.Once);
        }

        #endregion


        #region Deletar Tarefa

        [Fact(DisplayName = "Deletar Tarefa: Deve excluir a tarefa quando pertencer ao usuário e IDs são válidos")]
        public async Task DeletarTarefaDeveExcluirTarefaQuandoPertenceAoUsuario()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.DeletarTarefaAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            await tarefaService.DeletarTarefaAsync(idUsuario, idTarefa);

            // Assert
            iUnitOfWorkMock.Verify(u => u.TarefaRepository.DeletarTarefaAsync(It.IsAny<int>()), Times.Once);

            iUnitOfWorkMock.Verify(u => u.SalvarBancoAsync(), Times.Once);
        }


        [Fact(DisplayName = "Deletar Tarefa: Deve lançar InvalidOperationException quando a tarefa não pertence ao usuário")]
        public async Task DeletarTarefaDeveLancarExcecaoQuandoTarefaNaoPertenceAoUsuario()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tarefaService.DeletarTarefaAsync(idUsuario, idTarefa));
        }

        #endregion


        #region Alterar Tarefa

        [Fact(DisplayName = "Alterar Tarefa: Deve alterar a tarefa quando pertencer ao usuário e dados são válidos")]
        public async Task AlterarTarefaDeveAlterarTarefaQuandoPertenceAoUsuario()
        {
            // Arrange
            int idUsuario = 1;

            TarefaAlterarDTO dadosTarefaMock = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.AlterarTarefaAsync(It.IsAny<TbTarefa>())).Returns(Task.CompletedTask);

            // Act
            await tarefaService.AlterarTarefaAsync(idUsuario, dadosTarefaMock, "Alta", "Pendente");

            // Assert
            iUnitOfWorkMock.Verify(u => u.TarefaRepository.AlterarTarefaAsync(It.IsAny<TbTarefa>()), Times.Once);

            iUnitOfWorkMock.Verify(u => u.SalvarBancoAsync(), Times.Once);
        }


        [Fact(DisplayName = "Alterar Tarefa: Deve lançar InvalidOperationException quando a tarefa não pertence ao usuário")]
        public async Task AlterarTarefaDeveLancarExcecaoQuandoTarefaNaoPertenceAoUsuario()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            TarefaAlterarDTO dadosTarefaMock = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tarefaService.AlterarTarefaAsync(idUsuario, dadosTarefaMock, "Alta", "Pendente"));
        }

        #endregion


        #region Listar Tarefa Por Id Usuario

        [Fact(DisplayName = "Listar Tarefas: Deve listar tarefas Id do usuario e retornar uma ListaDTO")]
        public async Task ListaTarefasIdUsuarioComTarefasRetornaListaDTO()
        {
            // Arrange
            int idUsuario = 1;

            List<TbTarefa> listaTarefa = TarefaTestDataFactory.CriarDadosListaTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.ListaTarefasIdAsync(It.IsAny<int>())).ReturnsAsync(listaTarefa);

            // Act
            List<TarefaConsultaDTO> resultado = await tarefaService.ListaTarefasIdAsync(idUsuario);

            // Assert
            Assert.Equal(listaTarefa.Count, resultado.Count);
            Assert.All(resultado, dto => Assert.IsType<TarefaConsultaDTO>(dto));
        }


        [Fact(DisplayName = "Listar Tarefas: Deve lancar excecao por nao encontrar tarefas")]
        public async Task ListaTarefasIdUsuarioSemTarefasLancaExcecao()
        {
            // Arrange
            int idUsuario = 1;

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.ListaTarefasIdAsync(It.IsAny<int>())).ReturnsAsync(new List<TbTarefa>());

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => tarefaService.ListaTarefasIdAsync(idUsuario));
        }

        #endregion


        #region Listar Tarefa Paginada Por Status


        [Fact(DisplayName = "Listar Tarefas Paginadas: Deve retornar tarefas paginadas quando o status é nulo ou todas (todas as tarefas)")]
        public async Task ListarTarefasPaginadasStatusNuloDeveRetornarTodasAsTarefasPaginadas()
        {
            // Arrange
            int idUsuario = 1;
            int pageNumber = 1;
            int pageSize = 10;

            (List<TbTarefa> Items, int TotalCount) listaTarefa = TarefaTestDataFactory.CriarDadosListaTarefaPaginadaPorEstatus();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.ListaTarefasPaginadasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(listaTarefa);

            // Act
            var resultado = await tarefaService.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize, null);

            // Assert
            Assert.Equal(listaTarefa.TotalCount, resultado.TotalCount);
            Assert.All(resultado.Items, item => Assert.IsType<TarefaConsultaDTO>(item));
            iUnitOfWorkMock.Verify(u => u.TarefaRepository.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize), Times.Once); 
        }


        [Fact(DisplayName = "Listar Tarefas Paginadas: Deve retornar tarefas paginadas quando o status é diferente de nulo ou todas")]
        public async Task ListarTarefasPaginadasStatusDiferenteNuloDeveRetornarAsTarefasPaginadas()
        {
            // Arrange
            int idUsuario = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string status = "Pendente";

            (List<TbTarefa> Items, int TotalCount) listaTarefa = TarefaTestDataFactory.CriarDadosListaTarefaPaginadaPorEstatus();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.ListaTarefasPaginadasStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(listaTarefa);

            // Act
            var resultado = await tarefaService.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize, status);

            // Assert
            Assert.Equal(listaTarefa.TotalCount, resultado.TotalCount);
            Assert.All(resultado.Items, item => Assert.IsType<TarefaConsultaDTO>(item));
            iUnitOfWorkMock.Verify(u => u.TarefaRepository.ListaTarefasPaginadasStatusAsync(idUsuario, pageNumber, pageSize, status.Replace("_", " ")), Times.Once);
        }


        #endregion


        #region Buscar Tarefa

        [Fact(DisplayName = "Buscar Tarefa: Deve retornar TarefaConsultaDTO quando a tarefa existe e pertence ao usuário")]
        public async Task BuscarTarefaTarefaValidaDeveRetornarDTO()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            TbTarefa dadosTarefaMock = TarefaTestDataFactory.CriarDadosBuscarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            
            iUnitOfWorkMock.Setup(u => u.TarefaRepository.BuscarTarefaAsync(It.IsAny<int>())).ReturnsAsync(dadosTarefaMock);
            
            // Act
            var resultado = await tarefaService.BuscarTarefaAsync(idUsuario, idTarefa);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<TarefaConsultaDTO>(resultado); 
            Assert.Equal(dadosTarefaMock.TaTitulo, resultado.Titulo); 
            Assert.Equal(dadosTarefaMock.TaDescricao, resultado.Descricao); 
        }

        [Fact(DisplayName = "Buscar Tarefa: Deve lançar InvalidOperationException quando a tarefa não pertence ao usuário")]
        public async Task BuscarTarefaTarefaNaoAutorizadaDeveLancarExcecao()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => tarefaService.BuscarTarefaAsync(idUsuario, idTarefa));
        }

        [Fact(DisplayName = "Buscar Tarefa: Deve retornar TarefaConsultaDTO quando a tarefa existe e pertence ao usuário")]
        public async Task BuscarTarefaTarefaValidaDeveRetornarDTO1()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 999;

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            iUnitOfWorkMock
                .Setup(u => u.TarefaRepository.BuscarTarefaAsync(idTarefa)).ReturnsAsync((TbTarefa)null); 

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => tarefaService.BuscarTarefaAsync(idTarefa, idUsuario)
            );
        }

        #endregion


        #region Buscar QTD Status

        [Fact(DisplayName = "Buscar Quantidade Tarefa por Estatus: Deve retornar a quantidade de tarefadas e pertence ao usuário")]
        public async Task BuscarQtdStatus()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            var (pendente, emAndamento, concluido)  = TarefaTestDataFactory.CriarDadosQtdStatus();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.BuscarQtdStatusTarefaAsync(It.IsAny<int>())).ReturnsAsync((pendente, emAndamento, concluido));

            // Act
            var resultado = await tarefaService.BuscarQtdStatusEPorcentagemConclusaoAsync(idUsuario);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<TarefaQtdStatusDTO>(resultado);
        }

        #endregion
    }
}
