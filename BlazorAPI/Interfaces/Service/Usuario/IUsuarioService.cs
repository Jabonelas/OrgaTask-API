using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;

namespace BlazorAPI.Interfaces.Service.Usuario
{
    public interface IUsuarioService
    {
        Task CadastrarUsuarioAsync(UsuarioCadastrarDTO _dadosCadastroUsuario);

        Task<bool> LoginExisteAsync(string _login);

        Task<int> BuscarIdUsuarioAsync(string _login);

        Task LoginSenhaValidosAsync(UsuarioLoginDTO _dadosLogin);

        Task<UserToken> GerarTorkenAsync(int _idUsuario, UsuarioLoginDTO _dadosLogin);
    }
}