using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Autenticacao;
using BlazorAPI.Interfaces.Repository;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Models;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace BlazorAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository iUsuarioRepository;
        private readonly IAutenticacao iAutenticacao;

        public UsuarioService(IUsuarioRepository _iUsuarioRepository, IAutenticacao _iAutenticacao)
        {
            iUsuarioRepository = _iUsuarioRepository;

            iAutenticacao = _iAutenticacao;
        }

        public async Task CadastrarUsuarioAsync(UsuarioCadastrarDTO _dadosCadastroUsuario)
        {
            if (await LoginExisteAsync(_dadosCadastroUsuario.login.ToString()))
            {
                throw new InvalidOperationException("Login já cadastrado.");
            }

            TbUsuario usuario = MapearParaUsuario(_dadosCadastroUsuario);

            await iUsuarioRepository.AdicionarAsync(usuario);
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

        private TbUsuario MapearParaUsuario(UsuarioCadastrarDTO _dadosCadastroUsuario)
        {
            string senhaCriptografada = CriptografarSenha(_dadosCadastroUsuario.senha);

            TbUsuario usuario = new TbUsuario
            {
                UsNome = _dadosCadastroUsuario.nome,
                UsLogin = _dadosCadastroUsuario.login,
                UsSenha = senhaCriptografada
            };

            return usuario;
        }

        public async Task<bool> LoginExisteAsync(string _login)
        {
            return await iUsuarioRepository.LoginExisteAsync(_login);
        }

        public async Task LoginSenhaValidosAsync(UsuarioLoginDTO _dadosUsuarioLogin)
        {
            TbUsuario usuarioLogin = MapearParaUsuarioLogin(_dadosUsuarioLogin);

            if (!await iUsuarioRepository.LoginSenhaValidosAsync(usuarioLogin))
            {
                throw new UnauthorizedAccessException("Falha na autenticação: credenciais inválidas.");
            }
        }

        public async Task<int> BuscarIdUsuarioAsync(string _login)
        {
            return await iUsuarioRepository.BuscarIdUsuarioAsync(_login);
        }

        private TbUsuario MapearParaUsuarioLogin(UsuarioLoginDTO _dadosUsuarioLogin)
        {
            string senhaCriptografada = CriptografarSenha(_dadosUsuarioLogin.senha);

            TbUsuario usuario = new TbUsuario
            {
                UsNome = "",
                UsLogin = _dadosUsuarioLogin.login,
                UsSenha = senhaCriptografada
            };

            return usuario;
        }

        public async Task<UserToken> GerarTorkenAsync(UsuarioLoginDTO _dadosUsuarioLogin)
        {
            var token = iAutenticacao.GenerateToken(_dadosUsuarioLogin.login, _dadosUsuarioLogin.senha);

            return new UserToken
            {
                Token = token
            };
        }
    }
}