using BlazorAPI.Models;
using BlazorAPI.Repository.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrgaTask_API.Tests;
using Xunit;

namespace Repository
{
    public class UsuarioRepositoryTest
    {
        private BlazorAPIBancodbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<BlazorAPIBancodbContext>()
                .UseInMemoryDatabase(databaseName: "BlazorAPIDB_Test")
                .Options;

            return new BlazorAPIBancodbContext(options);
        }


        #region Adicionar Usuario

        [Fact(DisplayName = "Adicionar Usuario: deve adicionar usuário ao contexto")]
        public async Task AdicionarUsuarioDeveAdicionarUsuario()
        {
            // Arrange
            var contexto = CriarContextoInMemory();
            var repository = new UsuarioRepository(contexto);

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            // Act
            await repository.AdicionarAsync(dadosUsuarioMock);
            await contexto.SaveChangesAsync();

            // Assert
            var usuarioSalvo = await contexto.TbUsuarios.FirstOrDefaultAsync(u => u.IdUsuario == dadosUsuarioMock.IdUsuario);

            Assert.NotNull(usuarioSalvo);
            Assert.Equal(dadosUsuarioMock.UsNome, usuarioSalvo.UsNome);
        }

        #endregion


        #region Login Existe

        [Fact(DisplayName = "Login Existe: Deve retornar true se o login existir")]
        public async Task LoginExisteDeveRetornarTrueSeLoginExistir()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            contexto.TbUsuarios.Add(dadosUsuarioMock);

            await contexto.SaveChangesAsync();

            var repository = new UsuarioRepository(contexto);


            // Act
            bool resultado = await repository.LoginExisteAsync(dadosUsuarioMock.UsLogin);

            // Assert
            Assert.True(resultado);
        }


        [Fact(DisplayName = "Login Existe: Deve retornar false se o login nao existir")]
        public async Task LoginExisteDeveRetornarFalseSeLoginNaoExistir()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            contexto.TbUsuarios.Add(dadosUsuarioMock);

            await contexto.SaveChangesAsync();

            var repository = new UsuarioRepository(contexto);

            string usuario = "Usuario Invalido";

            // Act
            bool resultado = await repository.LoginExisteAsync(usuario);

            // Assert
            Assert.False(resultado);
        }

        #endregion


        #region Buscar Id Usuario

        [Fact(DisplayName = "Buscar Id Usuario: Deve retornar o id quando usuario existir")]
        public async Task BuscarIdUsuarioQuandoUsuarioValidoDeveRetornarIdUsuario()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            contexto.TbUsuarios.Add(dadosUsuarioMock);

            await contexto.SaveChangesAsync();

            var repository = new UsuarioRepository(contexto);

            // Act
            int resultado = await repository.BuscarIdUsuarioAsync(dadosUsuarioMock.UsLogin);

            // Assert
            Assert.NotEqual(0, resultado);
        }

        [Fact(DisplayName = "Buscar Id Usuario: Deve retornar 0 quando usuario nao existir")]
        public async Task BuscarIdUsuarioQuandoUsuarioInvalidoDeveRetornarZero()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            contexto.TbUsuarios.Add(dadosUsuarioMock);

            await contexto.SaveChangesAsync();

            var repository = new UsuarioRepository(contexto);

            string usuario = "Usuario Invalido";

            // Act
            int resultado = await repository.BuscarIdUsuarioAsync(usuario);

            // Assert
            Assert.Equal(resultado, 0);
        }

        #endregion


        #region Login Senha

        [Fact(DisplayName = "Login Senha: Deve retornar true quando usuário e senha são válidos")]
        public async Task LoginSenhaValidosQuandoCredenciaisSaoValidasDeveRetornarTrue()
        {
            // Arrange
            var contexto = CriarContextoInMemory();

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            contexto.TbUsuarios.Add(dadosUsuarioMock);

            await contexto.SaveChangesAsync();

            var repository = new UsuarioRepository(contexto);

            // Act
            bool resultado = await repository.LoginSenhaValidosAsync(dadosUsuarioMock);

            // Assert
            Assert.True(resultado);
        }


        [Fact(DisplayName = "Login Senha: Deve retornar false quando usuário e senha não são válidos")]
        public async Task LoginSenhaInvalidosQuandoCredenciaisNaoSaoValidasDeveRetornarFalse()
        {
            // Arrange
            var contexto = CriarContextoInMemory();
            var repository = new UsuarioRepository(contexto);

            TbUsuario dadosUsuarioMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuario();

            TbUsuario dadosUsuarioInvalidoMock = UsuarioTestDataFactory.CriarDadosCadastrarUsuarioInvalido();

            // Act
            bool resultado = await repository.LoginSenhaValidosAsync(dadosUsuarioInvalidoMock);

            // Assert
            Assert.False(resultado);
        }

        #endregion
    }
}
