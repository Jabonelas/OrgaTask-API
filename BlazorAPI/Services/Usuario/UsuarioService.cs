using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Service.Usuario;
using BlazorAPI.Interfaces.Unit_Of_Work;
using BlazorAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlazorAPI.Services.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IAutenticacao iAutenticacao;

        private readonly IUnitOfWork iUnitOfWork;

        public UsuarioService(IUnitOfWork _unitOfWork, IAutenticacao _iAutenticacao)
        {
            iUnitOfWork = _unitOfWork;
            iAutenticacao = _iAutenticacao;
        }

        public async Task CadastrarUsuarioAsync(UsuarioCadastrarDTO _dadosCadastroUsuario)
        {
            _dadosCadastroUsuario.Login = _dadosCadastroUsuario.Login.TrimEnd().ToLower();

            if (await LoginExisteAsync(_dadosCadastroUsuario.Login))
            {
                throw new InvalidOperationException("Login já cadastrado.");
            }

            TbUsuario usuario = _dadosCadastroUsuario;

            usuario.UsSenha = CriptografarSenha(_dadosCadastroUsuario.Senha);

            await iUnitOfWork.UsuarioReposity.AdicionarAsync(usuario);
            await iUnitOfWork.SalvarBancoAsync();
        }

        public string CriptografarSenha(string _senhaDigitada)
        {
            string senhaCriptografada;

            byte[] senhaBytes = Encoding.UTF8.GetBytes(_senhaDigitada);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] senhaHash = sha256.ComputeHash(senhaBytes);

                senhaCriptografada = Convert.ToBase64String(senhaHash);
            }

            return senhaCriptografada;
        }

        public async Task<bool> LoginExisteAsync(string _login)
        {
            return await iUnitOfWork.UsuarioReposity.LoginExisteAsync(_login);
        }

        public async Task LoginSenhaValidosAsync(UsuarioLoginDTO _dadosUsuarioLogin)
        {
            TbUsuario usuarioLogin = _dadosUsuarioLogin;

            usuarioLogin.UsSenha = CriptografarSenha(_dadosUsuarioLogin.Senha);

            usuarioLogin.UsLogin = usuarioLogin.UsLogin.TrimEnd().ToLower();

            if (!await iUnitOfWork.UsuarioReposity.LoginSenhaValidosAsync(usuarioLogin))
            {
                throw new UnauthorizedAccessException("Falha na autenticação: credenciais inválidas.");
            }

        }

        public async Task<int> BuscarIdUsuarioAsync(string _login)
        {
            _login = _login.TrimEnd();

            return await iUnitOfWork.UsuarioReposity.BuscarIdUsuarioAsync(_login);
        }

        public async Task<UserToken> GerarTokenAsync(int _idUsuario, UsuarioLoginDTO _dadosUsuarioLogin)
        {
            _dadosUsuarioLogin.Login = _dadosUsuarioLogin.Login.TrimEnd();

            var token = iAutenticacao.GenerateToken(_idUsuario, _dadosUsuarioLogin.Login);

            return new UserToken
            {
                Token = token
            };
        }
    }
}