using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Usuario;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Models;
using BlazorAPI.Responses;
using BlazorAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService iUsuarioService;

        public UsuarioController(IUsuarioService _iUsuarioService)
        {
            iUsuarioService = _iUsuarioService;
        }

        /// <summary>
        /// Cadastra um novo usuário no sistema
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /usuarios
        ///     {
        ///        "nome": "João Silva",
        ///        "login": "joao",
        ///        "senha": "SuaSenhaForte@123",
        ///     }
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     {
        ///        "message": "Usuário cadastrado com sucesso!"
        ///     }
        ///
        /// </remarks>
        /// <param name="_dadosUsuarioCadastro">DTO com os dados necessários para cadastro</param>
        /// <returns>Mensagem de sucesso ou erro detalhado</returns>
        /// <response code="201">Usuário cadastrado com sucesso</response>
        /// <response code="400">Se os dados forem inválidos</response>
        /// <response code="409">Se houver conflito na operação (ex: Login já cadastrado)</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost("cadastrar")]
        [ProducesResponseType(typeof(UsuarioCadastrarDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Cadastrar(UsuarioCadastrarDTO _dadosCadastroUsuario)
        {
            try
            {
                await iUsuarioService.CadastrarUsuarioAsync(_dadosCadastroUsuario);

                return Created("", new ErrorResponse { message = "Usuário cadastrado com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao cadastrar usuário." });
            }
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /login
        ///     {
        ///        "login": "joão",
        ///        "senha": "SuaSenhaSegura123"
        ///     }
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     {
        ///        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        ///     }
        ///
        /// </remarks>
        /// <param name="_usuarioLogin">DTO contendo login e senha do usuário</param>
        /// <returns>Token JWT para autenticação</returns>
        /// <response code="200">Retorna o token JWT gerado</response>
        /// <response code="400">Se as credenciais forem inválidas ou estiverem em formato incorreto</response>
        /// <response code="401">Se a autenticação falhar (login/senha incorretos)</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UsuarioLoginDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserToken>> UsuarioLogin(UsuarioLoginDTO _usuarioDadosLogin)
        {
            try
            {
                await iUsuarioService.LoginSenhaValidosAsync(_usuarioDadosLogin);

                var idUsuario = await iUsuarioService.BuscarIdUsuarioAsync(_usuarioDadosLogin.login);

                UserToken token = await iUsuarioService.GerarTorkenAsync(idUsuario, _usuarioDadosLogin);

                return token;
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao realizar login." });
            }
        }
    }
}