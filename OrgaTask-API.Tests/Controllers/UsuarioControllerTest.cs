using BlazorAPI.Controllers.Usuario;
using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Service.Usuario;
using BlazorAPI.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace OrgaTask_API.Tests.Controllers
{
    public class UsuarioControllerTest
    {
        private readonly Mock<IUsuarioService> iUsuarioServiceMock;
        private readonly UsuarioController usuarioController;

        public UsuarioControllerTest()
        {
            iUsuarioServiceMock = new Mock<IUsuarioService>();
            usuarioController = new UsuarioController(iUsuarioServiceMock.Object);
        }


        #region Cadastra Usuario

        [Fact(DisplayName = "Cadastrar Usuario: Com dados validos deve retornar Created 201")]
        public async Task CadastrarUsuarioComDadosValidosDeveRetornarCreated201()
        {
            // Arrange
            UsuarioCadastrarDTO dadosUsuario = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock.Setup(s => s.CadastrarUsuarioAsync(dadosUsuario)).Returns(Task.CompletedTask);

            // Act
            var result = await usuarioController.CadastrarUsuarioAsync(dadosUsuario);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            string mensagemRetorno = "Usuário cadastrado com sucesso!";

            var response = Assert.IsType<Response>(createdResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }


        [Fact(DisplayName = "Cadastrar Usuario: Quando usuário já cadastrado, deve retornar StatusCode 409")]
        public async Task CadastrarUsuarioComUsuarioJaCadastradoDeveRetornarStatusCode409()
        {
            // Arrange
            UsuarioCadastrarDTO dadosUsuario = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock.Setup(s => s.CadastrarUsuarioAsync(It.IsAny<UsuarioCadastrarDTO>())).ThrowsAsync(new InvalidOperationException("Login já cadastrado."));

            // Act 
            var result = await usuarioController.CadastrarUsuarioAsync(dadosUsuario);

            // Assert
            var statusCodeResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, statusCodeResult.StatusCode);

            string mensagemRetorno = "Login já cadastrado.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }



        [Fact(DisplayName = "Cadastrar Usuario: Com erro na service deve Retornar StatusCode 500")]
        public async Task CadastrarUsuarioComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            UsuarioCadastrarDTO dadosUsuario = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock.Setup(s => s.CadastrarUsuarioAsync(It.IsAny<UsuarioCadastrarDTO>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await usuarioController.CadastrarUsuarioAsync(dadosUsuario);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao cadastrar usuário.";

            var response = Assert.IsType<Response>(statusCodeResult.Value);
            Assert.Equal(mensagemRetorno, response.message);
        }

        #endregion



        #region Login

        [Fact(DisplayName = "Login: Com dados válidos deve retornar UserToken")]
        public async Task UsuarioLoginComDadosValidosDeveRetornarUserToken()
        {
            // Arrange
            var idUsuario = 1;

            var dadosUsuario = UsuarioTestDataFactory.CriarDadosLogin();

            var tokenEsperado = UsuarioTestDataFactory.CriarToken();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock
                .Setup(s => s.LoginSenhaValidosAsync(It.IsAny<UsuarioLoginDTO>()))
                .Returns(Task.CompletedTask);

            iUsuarioServiceMock
                .Setup(s => s.BuscarIdUsuarioAsync(It.IsAny<string>()))
                .ReturnsAsync(idUsuario);

            iUsuarioServiceMock
                .Setup(s => s.GerarTokenAsync(idUsuario, It.IsAny<UsuarioLoginDTO>()))
                .ReturnsAsync(tokenEsperado);

            // Act
            var result = await usuarioController.UsuarioLoginAsync(dadosUsuario);

            // Assert
            var tokenRetornado = Assert.IsType<UserToken>(result.Value);
            Assert.Equal(tokenEsperado.Token, tokenRetornado.Token);
        }


        [Fact(DisplayName = "Login: Com credenciais inválidas deve retornar Unauthorized 401")]
        public async Task UsuarioLoginComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var dadosUsuario = UsuarioTestDataFactory.CriarDadosLogin();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock
                .Setup(s => s.LoginSenhaValidosAsync(It.IsAny<UsuarioLoginDTO>()))
                .ThrowsAsync(new UnauthorizedAccessException("Falha na autenticação: credenciais inválidas."));

            // Act
            var result = await usuarioController.UsuarioLoginAsync(dadosUsuario);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<Response>(unauthorizedResult.Value);

            Assert.Equal("Falha na autenticação: credenciais inválidas.", response.message);
        }


        [Fact(DisplayName = "Login: Com erro na service deve Retornar StatusCode 500")]
        public async Task LoginComErroNaServiceDeveRetornarStatusCode500()
        {
            // Arrange
            var dadosUsuario = UsuarioTestDataFactory.CriarDadosLogin();

            ConfigurarControllerComUsuario();

            iUsuarioServiceMock.Setup(s => s.LoginSenhaValidosAsync(It.IsAny<UsuarioLoginDTO>())).Returns(Task.CompletedTask);

            iUsuarioServiceMock.Setup(s => s.BuscarIdUsuarioAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            // Act 
            var result = await usuarioController.UsuarioLoginAsync(dadosUsuario);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            string mensagemRetorno = "Erro interno ao realizar login.";

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

            usuarioController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}
