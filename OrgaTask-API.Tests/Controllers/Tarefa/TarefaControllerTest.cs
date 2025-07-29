using BlazorAPI.Controllers.Tarefa;
using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrgaTask_API.Tests;
using System.Security.Claims;
using Xunit;
using static BlazorAPI.Controllers.Tarefa.TarefaController;


namespace Controllers.Tarefa
{
    public class TarefaControllerTest
    {
        private readonly Mock<ITarefaService> iTarefaServiceMock;
        private readonly TarefaController tarefaController;

        public TarefaControllerTest()
        {
            iTarefaServiceMock = new Mock<ITarefaService>();
            tarefaController = new TarefaController(iTarefaServiceMock.Object);
        }

        #region Cadastrar

        [Fact(DisplayName = "Cadastrar Tarefa: Com dados validos deve retornar Created 201")]
        public async Task CadastrarTarefaComDadosValidosDeveRetornarCreated201()
        {
            // Arrange
            int idUsuario = 1;

            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.CadastrarTarefaAsync(idUsuario, It.IsAny<TarefaCadastrarDTO>(), PrioridadeTarefa.Média.ToString(), StatusTarefa.Concluído.ToString())).Returns(Task.CompletedTask);

            // Act
            var result = await tarefaController.CadastrarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            string mensagemRetorno = "Tarefa cadastrada com sucesso!";

            var response = Assert.IsType<Response>(createdResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Cadastrar Tarefa: Com prioridade invalida Retornar Badrequest 400")]
        public async Task CadastrarTarefaComPrioridadeInvalidaRetornaBadRequest400()
        {
            // Arrange
            ConfigurarControllerComUsuario();

            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            iTarefaServiceMock.Setup(s => s.CadastrarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaCadastrarDTO>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.CadastrarTarefaAsync(dadosTarefa, null, StatusTarefa.Pendente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "Prioridade inválida. Valores aceitos: Alta, Média, Baixa";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Cadastrar Tarefa: Com status invalido Retornar Badrequest 400")]
        public async Task CadastrarTarefaComStatusInvalidoRetornaBadRequest400()
        {
            // Arrange
            ConfigurarControllerComUsuario();

            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            iTarefaServiceMock.Setup(s => s.CadastrarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaCadastrarDTO>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.CadastrarTarefaAsync(dadosTarefa, PrioridadeTarefa.Alta, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "Status inválido. Valores aceitos: Pendente, Em_Progresso, Concluído, Todas";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Cadastrar tarefa: Usuário não autorizado retorna 401")]
        public async Task CadastrarTarefa_UsuarioNaoAutorizado_Retorna401()
        {
            // Arrange
            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.CadastrarTarefaAsync(
                    It.IsAny<int>(),
                    It.IsAny<TarefaCadastrarDTO>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.CadastrarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Cadastrar Tarefa: Com erro na service deve Retornar StatusCode 500")]
        public async Task CadastrarTarefaComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idUsuario = 1;

            TarefaCadastrarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosCadastrarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.CadastrarTarefaAsync(idUsuario, It.IsAny<TarefaCadastrarDTO>(), PrioridadeTarefa.Média.ToString(), StatusTarefa.Concluído.ToString())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.CadastrarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao cadastrar tarefa.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion


        #region Alterar

        [Fact(DisplayName = "Alterar Tarefa: Com dados validos Deve Retornar Ok 200")]
        public async Task AlterarTarefaComDadosValidosDeveRetornarOk200()
        {
            // Arrange
            int idUsuario = 1;

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(idUsuario, It.IsAny<TarefaAlterarDTO>(), PrioridadeTarefa.Média.ToString(), StatusTarefa.Concluído.ToString()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var updateResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, updateResult.StatusCode);

            string mensagemRetorno = "Tarefa alterada com sucesso!";

            var response = Assert.IsType<Response>(updateResult.Value);
            Assert.Equal(mensagemRetorno, response.message);

        }


        [Fact(DisplayName = "Alterar Tarefa: Com prioridade invalida Retornar Badrequest 400")]
        public async Task AlterarTarefaComPrioridadeInvalidaRetornaBadRequest400()
        {
            // Arrange
            ConfigurarControllerComUsuario();

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaAlterarDTO>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, null, StatusTarefa.Pendente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "Prioridade inválida. Valores aceitos: Alta, Média, Baixa";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Alterar Tarefa: Com status invalido Retornar Badrequest 400")]
        public async Task AlterarTarefaComStatusInvalidoRetornaBadRequest400()
        {
            // Arrange
            ConfigurarControllerComUsuario();

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaAlterarDTO>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Alta, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "Status inválido. Valores aceitos: Pendente, Em_Progresso, Concluído, Todas";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Alterar tarefa: Usuário não autorizado retorna 401")]
        public async Task AlterarTarefaUsuarioNaoAutorizadoRetorna401()
        {
            // Arrange
            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaAlterarDTO>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Alterar tarefa: Tarefa nao encontrada retorna NotFound 404")]
        public async Task AlterarTarefaTarefaNaoEncontradaRetornaNotFound404()
        {
            // Arrange
            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaAlterarDTO>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException("Tarefa não encontrada"));

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            string mensagemRetorno = "Tarefa não encontrada";

            var response = Assert.IsType<Response>(notFoundResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        [Fact(DisplayName = "Alterar tarefa: Operação não permitida retorna 422")]
        public async Task AlterarTarefaOperacaoNaoPermitidaRetorna422()
        {
            // Arrange
            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock
                .Setup(s => s.AlterarTarefaAsync(It.IsAny<int>(), It.IsAny<TarefaAlterarDTO>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Operação não permitida"));

            // Act
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(422, statusCodeResult.StatusCode);

            string mensagemRetorno = "Operação não permitida";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Alterar Tarefa: Com erro na service deve Retornar StatusCode 500")]
        public async Task AlterarTarefaComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idUsuario = 1;

            TarefaAlterarDTO dadosTarefa = TarefaTestDataFactory.CriarDadosAlterarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.AlterarTarefaAsync(idUsuario, It.IsAny<TarefaAlterarDTO>(), PrioridadeTarefa.Média.ToString(), StatusTarefa.Concluído.ToString())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.AlterarTarefaAsync(dadosTarefa, PrioridadeTarefa.Média, StatusTarefa.Concluído);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao alterar tarefa.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        #endregion


        #region Deletar


        [Fact(DisplayName = "Deletar tarefa: Com IdTarefa valido retorna OK 200")]
        public async Task DeletarTarefaComIdValidoRetornaOK200()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.DeletarTarefaAsync(idTarefa, idUsuario)).Returns(Task.CompletedTask);

            // Act
            var result = await tarefaController.DeletarTarefaAsync(idTarefa);

            // Assert
            var deleteResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, deleteResult.StatusCode);

            string mensagemRetorno = "Tarefa deletada com sucesso!";

            var response = Assert.IsType<Response>(deleteResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Deletar tarefa: Usuario nao autorizado retorna Unauthorized 401 ")]
        public async Task DeletarTarefaUsuarioNaoAutorizadoRetornaUnauthorized401()
        {
            // Arrange
            int idTarefa = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.DeletarTarefaAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.DeletarTarefaAsync(idTarefa);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Deletar tarefa: Operação não permitida retorna 422")]
        public async Task DeletarTarefaOperacaoNaoPermitidaRetorna422()
        {
            // Arrange
            int idTarefa = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.DeletarTarefaAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("Operação não permitida"));

            // Act
            var result = await tarefaController.DeletarTarefaAsync(idTarefa);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(422, statusCodeResult.StatusCode);

            string mensagemRetorno = "Operação não permitida";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Deletar Tarefa: Com erro na service deve Retornar StatusCode 500")]
        public async Task DeletarTarefaComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idUsuario = 1;


            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.DeletarTarefaAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.DeletarTarefaAsync(idUsuario);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao deletar tarefa.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion


        #region Listar Tarefas Paginadas

        [Fact(DisplayName = "Listar Tarefa Paginadas: Lista todas as tarefas associadas ao usuário retorna OK 200 com lista de taredas")]
        public async Task ListarTarefaPaginadasComIdValidoRetornaOK200()
        {
            // Arrange
            int idUsuario = 1;
            int pageNumber = 1;
            int pageSize = 12;
            var status = StatusTarefa.Pendente;

            ConfigurarControllerComUsuario();

            PagedResult<TarefaConsultaDTO> listaTarefaPagianda = TarefaTestDataFactory.CriarDadosListaTarefaPaginada();

            iTarefaServiceMock.Setup(s => s.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize, It.IsAny<string>())).ReturnsAsync(listaTarefaPagianda);

            // Act
            var actionResult = await tarefaController.ListaTarefasPaginadasAsync(status, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<PagedResult<TarefaConsultaDTO>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal("Tarefa 1", response.Items[0].Titulo);
            Assert.Equal("Tarefa 2", response.Items[1].Titulo);
        }


        [Fact(DisplayName = "ListarTarefasPaginadas: Parâmetros pageNumber e pageSize inválidos (negativos) retorna BadRequest 400")]
        public async Task ListarTarefasPaginadasParametrosInvalidosRetorna400()
        {
            // Arrange
            int pageNumber = -1;
            int pageSize = 12;
            var status = StatusTarefa.Pendente;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.ListaTarefasPaginadasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.ListaTarefasPaginadasAsync(status, pageNumber, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "Os parâmetros pageNumber e pageSize devem ser maiores que zero.";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "ListarTarefasPaginadas: Quando pageSize > 50 deve retornar BadRequest 400")]
        public async Task ListarTarefasPaginadas_PageSizeAcimaDoLimite_Retorna400()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 99;
            var status = StatusTarefa.Pendente;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.ListaTarefasPaginadasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await tarefaController.ListaTarefasPaginadasAsync(status, pageNumber, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);

            string mensagemRetorno = "O tamanho máximo por página é 50 itens.";

            var response = Assert.IsType<Response>(badRequestResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Listar Tarefa Paginadas: Usuario nao autorizado retorna Unauthorized 401 ")]
        public async Task ListarTarefaPaginadasUsuarioNaoAutorizadoRetornaUnauthorized401()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 12;
            var status = StatusTarefa.Pendente;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.ListaTarefasPaginadasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.ListaTarefasPaginadasAsync(status, pageNumber, pageSize);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Listar Tarefa Paginadas: Com erro na service deve Retornar StatusCode 500")]
        public async Task ListarTarefaPaginadasComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 12;


            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.ListaTarefasPaginadasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.ListaTarefasPaginadasAsync(StatusTarefa.Pendente, pageNumber, pageSize);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao buscar lista de tarefas paginadas. Database error";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion


        #region Buscar Tarefa por ID

        [Fact(DisplayName = "Buscar Tarefa Por ID: tarefa existente e valida retorna 200 com dados completos")]
        public async Task BuscarTarefaPorIDTarefaExistenteEValidaRetorna200ComDadosCompletos()
        {
            // Arrange
            int idUsuario = 1;
            int idTarefa = 1;

            TarefaConsultaDTO dadosTarefa = TarefaTestDataFactory.CriarDadosBuscarTarefa();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefaAsync(idTarefa, idUsuario)).ReturnsAsync(dadosTarefa);

            // Act
            var result = await tarefaController.BuscarTarefaAsync(idUsuario);

            // Assert
            var updateResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, updateResult.StatusCode);

            var response = Assert.IsType<TarefaConsultaDTO>(updateResult.Value);
            Assert.Equal(dadosTarefa.Id, response.Id);
            Assert.Equal(dadosTarefa.Titulo, response.Titulo);
            Assert.Equal(dadosTarefa.Descricao, response.Descricao);
            Assert.Equal(dadosTarefa.Prioridade, response.Prioridade);
            Assert.Equal(dadosTarefa.Prazo, response.Prazo);
            Assert.Equal(dadosTarefa.Status, response.Status);
            Assert.Equal(dadosTarefa.DataCriacao, response.DataCriacao);
        }


        [Fact(DisplayName = "Buscar Tarefa Por ID: Dado usuário não autorizado, quando buscar tarefa, retorna 401")]
        public async Task BuscarTarefaPorIDAcessoNaoAutorizadoRetorna401()
        {
            // Arrange
            int idTarefa = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefaAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.BuscarTarefaAsync(idTarefa);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Tarefa Por ID: Tarefa nao encontrada retorna NotFound 404")]
        public async Task BuscarTarefaPorIDTarefaNaoEncontradaRetornaNotFound404()
        {
            // Arrange
            int idTarefa = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefaAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Tarefa não encontrada"));

            // Act
            var result = await tarefaController.BuscarTarefaAsync(idTarefa);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

            string mensagemRetorno = "Tarefa não encontrada";

            var response = Assert.IsType<Response>(notFoundResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Tarefa Por ID: Com erro na service deve Retornar StatusCode 500")]
        public async Task BuscarTarefaPorIDComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idTarefa = 1;


            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefaAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.BuscarTarefaAsync(idTarefa);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao buscar tarefas.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        #endregion


        #region Buscar quantidade das tarefas por Status


        [Fact(DisplayName = "Buscar Qtd Status E Porcentagem Conclusao: Retorna OK 200 com estatísticas")]
        public async Task BuscarQtdStatusEPorcentagemConclusaoDadosValidosRetorna200ComEstatisticas()
        {
            // Arrange
            int idUsuario = 1;

            TarefaQtdStatusDTO dadosTarefa = TarefaTestDataFactory.CriarDadosBuscarQtdStatusEPorcentagemConclusao();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarQtdStatusEPorcentagemConclusaoAsync(idUsuario)).ReturnsAsync(dadosTarefa);

            // Act
            var result = await tarefaController.BuscarQtdStatusEPorcentagemConclusaoAsync();

            // Assert
            var updateResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, updateResult.StatusCode);

            var response = Assert.IsType<TarefaQtdStatusDTO>(updateResult.Value);
            Assert.Equal(dadosTarefa.Concluido, response.Concluido);
            Assert.Equal(dadosTarefa.EmProgresso, response.EmProgresso);
            Assert.Equal(dadosTarefa.Pendente, response.Pendente);
            Assert.Equal(dadosTarefa.PorcentagemConcluidas, response.PorcentagemConcluidas);
        }


        [Fact(DisplayName = "Buscar Qtd Status E Porcentagem Conclusao: Dado usuário não autorizado, quando buscar estatísticas, retorna 401")]
        public async Task BuscarQtdStatusEPorcentagemConclusaoAcessoNaoAutorizadoRetorna401()
        {
            // Arrange
            int idUsuario = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarQtdStatusEPorcentagemConclusaoAsync(It.IsAny<int>())).ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.BuscarQtdStatusEPorcentagemConclusaoAsync();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Qtd Status E Porcentagem Conclusao: Estatísticas nao encontrada retorna NotFound 404")]
        public async Task BuscarQtdStatusEPorcentagemConclusaoTarefaNaoEncontradaRetornaNotFound404()
        {
            // Arrange
            int idUsuario = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarQtdStatusEPorcentagemConclusaoAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("Tarefa não encontrada"));

            // Act
            var result = await tarefaController.BuscarQtdStatusEPorcentagemConclusaoAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

            string mensagemRetorno = "Tarefa não encontrada";

            var response = Assert.IsType<Response>(notFoundResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Qtd Status E Porcentagem Conclusao: Com erro na service deve Retornar StatusCode 500")]
        public async Task BuscarQtdStatusEPorcentagemConclusaoComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idUsuario = 1;


            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarQtdStatusEPorcentagemConclusaoAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.BuscarQtdStatusEPorcentagemConclusaoAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao buscar quantidade dos status das tarefas.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion


        #region Buscar Tarefas Prioridade Alta


        [Fact(DisplayName = "Buscar Tarefas Prioridade Alta: Deve retornar 200 OK com lista de tarefas de alta prioridade")]
        public async Task BuscarTarefasPrioridadeAltaQuandoExistiremRetorna200ComLista()
        {
            // Arrange
            int idUsuario = 1;

            List<TarefaPrioridadeAltaDTO> dadosTarefa = TarefaTestDataFactory.CriarDadosBuscarTarefasPrioridadeAlta();

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefasPrioridadeAltaAsync(idUsuario))
                .ReturnsAsync(dadosTarefa);

            // Act
            var result = await tarefaController.BuscarTarefasPrioridadeAltaAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<List<TarefaPrioridadeAltaDTO>>(okResult.Value);
            Assert.Equal(dadosTarefa.Count, response.Count);
            Assert.Equal(dadosTarefa[0].Id, response[0].Id);
            Assert.Equal(dadosTarefa[0].Titulo, response[0].Titulo);
            Assert.Equal(dadosTarefa[0].Data, response[0].Data);
            Assert.Equal(dadosTarefa[0].Status, response[0].Status);
            Assert.Equal(dadosTarefa[0].Prazo, response[0].Prazo);
        }


        [Fact(DisplayName = "Buscar Tarefas Prioridade Alta: Dado usuário não autorizado, quando buscar tarefas, retorna 401")]
        public async Task BuscarTarefasPrioridadeAltaAcessoNaoAutorizadoRetorna401()
        {
            // Arrange
            int idUsuario = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefasPrioridadeAltaAsync(It.IsAny<int>())).ThrowsAsync(new UnauthorizedAccessException("Usuário não autorizado"));

            // Act
            var result = await tarefaController.BuscarTarefasPrioridadeAltaAsync();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);

            string mensagemRetorno = "Usuário não autorizado";

            var response = Assert.IsType<Response>(unauthorizedResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Tarefas Prioridade Alta: Estatísticas nao encontrada retorna NotFound 404")]
        public async Task BuscarTarefasPrioridadeAltaTarefaNaoEncontradaRetornaNotFound404()
        {
            // Arrange
            int idUsuario = 1;

            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefasPrioridadeAltaAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("Tarefa não encontrada"));

            // Act
            var result = await tarefaController.BuscarTarefasPrioridadeAltaAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

            string mensagemRetorno = "Tarefa não encontrada";

            var response = Assert.IsType<Response>(notFoundResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Buscar Tarefas Prioridade Alta: Com erro na service deve Retornar StatusCode 500")]
        public async Task BuscarTarefasPrioridadeAltaComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            int idUsuario = 1;


            ConfigurarControllerComUsuario();

            iTarefaServiceMock.Setup(s => s.BuscarTarefasPrioridadeAltaAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await tarefaController.BuscarTarefasPrioridadeAltaAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao buscar tarefas com prioridade alta.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion


        private void ConfigurarControllerComUsuario()
        {
            int idUsuario = 1;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("idUsuario", idUsuario.ToString())
            }, "mock"));

            tarefaController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}