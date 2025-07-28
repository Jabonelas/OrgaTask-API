using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;

namespace OrgaTask_API.Tests
{
    public class UsuarioTestDataFactory
    {
        public static UsuarioCadastrarDTO CriarDadosCadastrarUsuario()
        {
            return new UsuarioCadastrarDTO()
            {
                Nome = "nome usaurio",
                Login = "usuario teste",
                Senha = "123456789"
            };
        }

        public static UsuarioLoginDTO CriarDadosLogin()
        {
            return new UsuarioLoginDTO()
            {
                Login = "usuario teste",
                Senha = "123456789"

            };
        }

        public static UserToken CriarToken()
        {
            return new UserToken
            {
                Token = "tokenteste123",
            };
        }
    }
}
