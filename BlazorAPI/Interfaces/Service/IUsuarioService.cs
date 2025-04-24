using BlazorAPI.DTOs;

namespace BlazorAPI.Interfaces.Service
{
    public interface IUsuarioService
    {
        Task CadastrarUsuarioAsync(UsuarioCadastrarDTO _dadosCadastroUsuario);

        Task<bool> LoginExisteAsync(string _login);

        Task LoginSenhaValidosAsync(UsuarioLoginDTO _dadosLogin);

        Task<UserToken> GerarTorkenAsync(UsuarioLoginDTO _dadosLogin);
    }
}