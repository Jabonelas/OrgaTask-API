using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service;
using BlazorAPI.Responses;
using BlazorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TarefaController : Controller
    {
        private readonly ITarefaService iTarefaService;

        public TarefaController(ITarefaService _iTarefaService)
        {
            iTarefaService = _iTarefaService;
        }

        /// <summary>
        /// Cadastra uma nova tarefa para um usuário específico
        /// </summary>
        /// <remarks>
        /// Requer autenticação via JWT.
        ///
        /// Exemplo de requisição:
        ///
        ///     POST /usuarios/tarefas
        ///     {
        ///        "titulo": "Implementar API",
        ///        "descricao": "Desenvolver endpoints da aplicação",
        ///        "status": "pendente"
        ///     }
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     "message": "Tarefa cadastrada com sucesso!"
        ///
        /// </remarks>
        /// <param name="_idUsuario">ID do usuário (número inteiro positivo) que receberá a tarefa</param>
        /// <param name="_dadosTarefaCadastro">DTO com os dados necessários para cadastro da tarefa</param>
        /// <returns>Mensagem de confirmação do cadastro</returns>
        /// <response code="201">Retorna mensagem de sucesso ao cadastrar a tarefa</response>
        /// <response code="400">Se os dados da tarefa forem inválidos</response>
        /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
        /// <response code="404">Se o usuário não for encontrado</response>
        /// <response code="422">Status inválido. Informe um dos seguintes: 'pendente', 'em andamento' ou 'concluído'</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost("cadastrar")]
        [ProducesResponseType(typeof(IEnumerable<TarefaCadastrarDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CadastrarTarefa(TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            try
            {
                // Recupera o ID do usuário do token
                var idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

                await iTarefaService.CadastrarTarefaAsync(idUsuario, _dadosTarefaCadastro);

                return Created("", new ErrorResponse { message = "Tarefa cadastrada com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(422, new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao cadastrar tarefa." });
            }
        }

        /// <summary>
        /// Altera uma tarefa existente com os dados fornecidos
        /// </summary>
        /// <remarks>
        /// Requer autenticação via JWT.
        ///
        /// Exemplo de requisição:
        ///
        ///     POST /usuarios/alterar
        ///     {
        ///        "id": 1,
        ///        "titulo": "Atualizar API",
        ///        "descricao": "Revisar e atualizar endpoints existentes",
        ///        "status": "em andamento"
        ///     }
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     "message": "Tarefa alterada com sucesso!"
        ///
        /// </remarks>
        /// <param name="_dadosTarefaCadastro">DTO com os dados necessários para alteração da tarefa</param>
        /// <returns>Mensagem de confirmação da alteração</returns>
        /// <response code="201">Retorna mensagem de sucesso ao alterar a tarefa</response>
        /// <response code="400">Se os dados da tarefa forem inválidos</response>
        /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
        /// <response code="404">Se a tarefa não for encontrada</response>
        /// <response code="422">Status inválido. Informe um dos seguintes: 'pendente', 'em andamento' ou 'concluído'</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPut("alterar")]
        [ProducesResponseType(typeof(IEnumerable<TarefaCadastrarDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AlterarTarefa(TarefaCadastrarDTO _dadosTarefaCadastro)
        {
            try
            {
                // Recupera o ID do usuário do token
                var idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

                await iTarefaService.AlterarTarefaAsync(_dadosTarefaCadastro, idUsuario);

                return Created("", new ErrorResponse { message = "Tarefa alterada com sucesso!" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(422, new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao alterar tarefa." });
            }
        }

        /// <summary>
        /// Deleta uma tarefa existente com base no ID fornecido.
        /// </summary>
        /// <remarks>
        /// Requer autenticação via JWT.
        ///
        /// Exemplo de requisição:
        ///
        ///     POST /tarefas/deletar?idTarefa=1
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     "message": "Tarefa deletada com sucesso!"
        ///
        /// </remarks>
        /// <param name="_idTarefa">ID da tarefa a ser deletada</param>
        /// <returns>Mensagem de confirmação da exclusão</returns>
        /// <response code="201">Retorna mensagem de sucesso ao deletar a tarefa</response>
        /// <response code="400">Se o ID da tarefa for inválido</response>
        /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
        /// <response code="404">Se a tarefa não for encontrada</response>
        /// <response code="422">Se a operação não puder ser concluída devido a regras de negócio</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpDelete("deletar/{_idTarefa}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaCadastrarDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletarTarefa(int _idTarefa)
        {
            try
            {
                // Recupera o ID do usuário do token
                var idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

                await iTarefaService.DeletarTarefaAsync(_idTarefa, idUsuario);

                return Created("", new ErrorResponse { message = "Tarefa deletada com sucesso!" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(422, new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao deletar tarefa." });
            }
        }

        /// <summary>
        /// Lista todas as tarefas de um usuário específico
        /// </summary>
        /// <remarks>
        /// Requer autenticação via JWT.
        ///
        /// Exemplo de requisição:
        ///
        ///     GET /tarefa/lista
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     [
        ///        {
        ///           "id": 1,
        ///           "titulo": "Implementar API",
        ///           "descricao": "Desenvolver endpoints da aplicação",
        ///           "status": "concluído"
        ///        },
        ///        {
        ///           "id": 2,
        ///           "titulo": "Criar documentação",
        ///           "descricao": "Documentar endpoints no Swagger",
        ///           "status": "pendente"
        ///        }
        ///     ]
        ///
        /// </remarks>
        /// <param name="_idUsuario">ID do usuário (número inteiro positivo)</param>
        /// <returns>Lista de tarefas do usuário no formato JSON</returns>
        /// <response code="200">Retorna a lista de tarefas do usuário</response>
        /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
        /// <response code="404">Se o usuário não for encontrado</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpGet("lista")]
        [ProducesResponseType(typeof(IEnumerable<TarefaConsultaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TarefaConsultaDTO>>> ListaTarefas()
        {
            try
            {
                // Recupera o ID do usuário do token
                var idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

                var listaTarefas = await iTarefaService.ListaTarefasIdAsync(idUsuario);

                return Ok(listaTarefas);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar lista tarefas." });
            }
        }

        /// <summary>
        /// Busca uma tarefa específica pelo seu ID
        /// </summary>
        /// <remarks>
        /// Requer autenticação via JWT.
        ///
        /// Exemplo de requisição:
        ///
        ///     GET /tarefa/1/buscar
        ///
        /// Exemplo de resposta de sucesso:
        ///
        ///     {
        ///        "titulo": "Implementar API",
        ///        "descricao": "Desenvolver endpoints da aplicação",
        ///        "prioridade": "Alta",
        ///        "prazo": 7,
        ///        "status": "Em andamento",
        ///        "dataCriacao": "2023-05-15T10:30:00",
        ///     }
        ///
        /// </remarks>
        /// <param name="_idTarefa">ID da tarefa (número inteiro positivo)</param>
        /// <returns>Dados completos da tarefa no formato JSON</returns>
        /// <response code="200">Retorna os dados da tarefa solicitada</response>
        /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
        /// <response code="404">Se nenhuma tarefa for encontrada com o ID fornecido</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpGet("{_idTarefa}/buscar")]
        [ProducesResponseType(typeof(IEnumerable<TarefaConsultaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TarefaConsultaDTO>> BuscarTarefaAsync(int _idTarefa)
        {
            try
            {
                // Recupera o ID do usuário do token
                var idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

                TarefaCadastrarDTO tarefa = await iTarefaService.BuscarTarefaAsync(_idTarefa, idUsuario);

                return Ok(tarefa);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar lista tarefas." });
            }
        }
    }
}