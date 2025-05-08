using BlazorAPI.Models;

namespace BlazorAPI.Interfaces.Repository
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(TbUsuario _dadosCadastroUsuairo);

        Task<bool> LoginExisteAsync(string _login);

        Task<int> BuscarIdUsuarioAsync(string _login);

        Task<bool> LoginSenhaValidosAsync(TbUsuario _dadosUsuarioLogin);
    }
}