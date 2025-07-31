using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;
using BlazorAPI.Services.Usuario;
using Moq;
using OrgaTask_API.Tests;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Services
{
    public class UsuarioServiceTest
    {
        private readonly Mock<IUnitOfWork> iUnitOfWorkMock;

        private readonly Mock<IAutenticacao> iAutenticacaoMock;

        private readonly UsuarioService usuarioService;

        public UsuarioServiceTest()
        {
            iUnitOfWorkMock = new Mock<IUnitOfWork>();
            iAutenticacaoMock = new Mock<IAutenticacao>();
            usuarioService = new UsuarioService(iUnitOfWorkMock.Object, iAutenticacaoMock.Object);
        }

        #region Cadastrar Usuario

        [Fact(DisplayName = "Cadastrar Usuario: Login novo deve salvar no banco")]
        public async Task CadastrarUsuarioAsyncLoginNovoDeveSalvarNoBanco()
        {
            // Arrange
            UsuarioCadastrarDTO dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.LoginExisteAsync(It.IsAny<string>())).ReturnsAsync(false);

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.AdicionarAsync(It.IsAny<TbUsuario>()));

            // Act
            await usuarioService.CadastrarUsuarioAsync(dadosUsuarioMock);

            // Assert
            iUnitOfWorkMock.Verify(u => u.UsuarioReposity.AdicionarAsync(It.IsAny<TbUsuario>()), Times.Once);

            iUnitOfWorkMock.Verify(u => u.SalvarBancoAsync(), Times.Once);
        }

        [Fact(DisplayName = "Cadastrar Usuario: Login existente deve lancar excecao")]
        public async Task CadastrarUsuarioAsyncLoginExistenteDeveLancarExcecao()
        {
            // Arrange
            UsuarioCadastrarDTO dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.LoginExisteAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => usuarioService.CadastrarUsuarioAsync(dadosUsuarioMock));
        }

        #endregion


        #region Login Existente

        [Theory]
        [InlineData("existente", true)]
        [InlineData("inexistente", false)]
        public async Task LoginExisteAsyncDeveRetornarResultadoCorreto(string _login, bool _esperado)
        {
            // Arrange
            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.LoginExisteAsync(_login)).ReturnsAsync(_esperado);

            // Act
            var resultado = await usuarioService.LoginExisteAsync(_login);

            // Assert
            Assert.Equal(_esperado, resultado);
        }

        #endregion


        #region Login Senha Validos

        [Fact(DisplayName = "Login Senha Validos: Credenciais corretas nao deve lancar excecao()")]
        public async Task LoginSenhaValidosAsyncCredenciaisCorretasNaoDeveLancarExcecao()
        {
            // Arrange
            UsuarioLoginDTO dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosLogin();

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.LoginSenhaValidosAsync(It.IsAny<TbUsuario>())).ReturnsAsync(true);

            // Act 
            await usuarioService.LoginSenhaValidosAsync(dadosUsuarioMock);
        }

        [Fact(DisplayName = "Login Senha Validos: Credenciais invalidas deve lancar excecao")]
        public async Task LoginSenhaValidosAsyncCredenciaisInvalidasDeveLancarExcecao()
        {
            // Arrange
            UsuarioLoginDTO dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosLogin();

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.LoginSenhaValidosAsync(It.IsAny<TbUsuario>())).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => usuarioService.LoginSenhaValidosAsync(dadosUsuarioMock));
        }

        #endregion


        #region Criptografar Senha

        [Fact(DisplayName = "Criptografar Senha: Deve retornar HashBase64")]
        public void CriptografarSenhaDeveRetornarHashBase64()
        {
            // Arrange
            string senha = "123";

            string hashEsperado = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(senha)));

            // Act
            var resultado = usuarioService.CriptografarSenha(senha);

            // Assert
            Assert.Equal(hashEsperado, resultado);
        }

        #endregion


        #region Buscar Id Usuario

        [Fact(DisplayName = "Buscar Id Usuario: Login existente deve retornar Id")]
        public async Task BuscarIdUsuarioLoginExistenteDeveRetornarId()
        {
            // Arrange
            int idEsperado = 1;

            string usuario = "usuario";

            iUnitOfWorkMock.Setup(u => u.UsuarioReposity.BuscarIdUsuarioAsync(It.IsAny<string>())).ReturnsAsync(idEsperado);

            // Act
            var resultado = await usuarioService.BuscarIdUsuarioAsync(usuario);

            // Assert
            Assert.Equal(idEsperado, resultado);
        }

        #endregion


        #region Gerar Token

        [Fact(DisplayName = "Gerar Token: Deve chamar gerador de Token")]
        public async Task GerarTokenDeveChamarGeradorDeToken()
        {
            // Arrange
            int idUsuario = 1;

            UsuarioLoginDTO dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosLogin();

            UserToken token = UsuarioTestDataFactory.CriarToken();

            iAutenticacaoMock.Setup(a => a.GenerateToken(idUsuario, It.IsAny<string>())).Returns(token.Token);

            // Act
            var resultado = await usuarioService.GerarTokenAsync(idUsuario, dadosUsuarioMock);

            // Assert
            Assert.Equal(token.Token, resultado.Token);
            iAutenticacaoMock.Verify(a => a.GenerateToken(idUsuario, It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
