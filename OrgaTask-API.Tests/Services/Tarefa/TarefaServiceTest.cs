using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;
using BlazorAPI.Services.Tarefa;
using BlazorAPI.Services.Usuario;
using Moq;
using OrgaTask_API.Tests;
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

            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.CadastrarTarefaAsync(It.IsAny<TbTarefa>())).Returns(Task.CompletedTask);

            iUnitOfWorkMock.Setup(u => u.SalvarBancoAsync());

            // Act
            await tarefaService.CadastrarTarefaAsync(idUsuario, dadosTarefa, "Alta", "Pendente");

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

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.AlterarTarefaAsync(It.IsAny<TbTarefa>())).Returns(Task.CompletedTask);

            // Act
            await tarefaService.AlterarTarefaAsync(idUsuario, dadosTarefa, "Alta", "Pendente");

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

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iUnitOfWorkMock.Setup(u => u.TarefaRepository.TarefaPertenceUsuarioAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => tarefaService.AlterarTarefaAsync(idUsuario, dadosTarefa, "Alta", "Pendente"));
        }

        #endregion
    }
}
