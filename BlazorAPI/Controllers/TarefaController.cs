using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAPI.Controllers;

[ApiController]
[Route("api/tarefas")]
public class TarefaController : ControllerBase
{
    //[Authorize]
    private readonly ITarefaService tarefaService;

    //private readonly ITarefaCacheService tarefacacheService;

    //public TarefaController(ITarefaService _tarefaService, ITarefaCacheService _tarefacacheService)
    //{
    //    tarefaService = _tarefaService;
    //    tarefacacheService = _tarefacacheService;
    //}

    public TarefaController(ITarefaService _tarefaService)
    {
        tarefaService = _tarefaService;
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
    /// <param name="dadosTarefaCadastro">DTO com os dados necessários para cadastro da tarefa</param>
    /// <returns>Mensagem de confirmação do cadastro</returns>
    /// <response code="201">Retorna mensagem de sucesso ao cadastrar a tarefa</response>
    /// <response code="400">Se os dados da tarefa forem inválidos</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Se o usuário não for encontrado</response>
    /// <response code="422">Status inválido. Informe um dos seguintes: 'pendente', 'em andamento' ou 'concluído'</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<TarefaDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CadastrarTarefa(TarefaDTO dadosTarefaCadastro)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            await tarefaService.CadastrarTarefaAsync(idUsuario, dadosTarefaCadastro);

            //await tarefacacheService.InvalidarCache(idUsuario);

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
        catch (Exception)
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
    [HttpPut]
    [ProducesResponseType(typeof(IEnumerable<TarefaDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AlterarTarefa(TarefaDTO _dadosTarefaCadastro)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            await tarefaService.AlterarTarefaAsync(_dadosTarefaCadastro, idUsuario);

            //await tarefacacheService.InvalidarCache(idUsuario);

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
        catch (Exception)
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
    /// <param name="id">ID da tarefa a ser deletada</param>
    /// <returns>Mensagem de confirmação da exclusão</returns>
    /// <response code="201">Retorna mensagem de sucesso ao deletar a tarefa</response>
    /// <response code="400">Se o ID da tarefa for inválido</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Se a tarefa não for encontrada</response>
    /// <response code="422">Se a operação não puder ser concluída devido a regras de negócio</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(IEnumerable<TarefaDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletarTarefa(int id)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            await tarefaService.DeletarTarefaAsync(id, idUsuario);

            //await tarefacacheService.InvalidarCache(idUsuario);

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
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse { message = "Erro interno ao deletar tarefa." });
        }
    }

    /// <summary>
    /// Lista todas as tarefas do usuário autenticado
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
    ///          "id": 1,
    ///          "titulo": "Implementar API",
    ///          "descricao": "Desenvolver endpoints da aplicação",
    ///          "prioridade": "Alta",
    ///          "prazo": 5,
    ///          "status": "Em Progresso",
    ///          "data": "2023-10-25T00:00:00"
    ///        },
    ///        {
    ///          "id": 2,
    ///          "titulo": "Criar documentação",
    ///          "descricao": "Documentar endpoints no Swagger",
    ///          "prioridade": "Média",
    ///          "prazo": 3,
    ///          "status": "Pendente",
    ///          "data": "2023-10-26T00:00:00"
    ///        }
    ///     ]
    /// </remarks>
    /// <returns>Lista de tarefas do usuário no formato JSON</returns>
    /// <response code="200">Retorna a lista de tarefas do usuário</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Nenhuma tarefa encontrada para o usuário</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaConsultaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TarefaConsultaDTO>>> ListaTarefas()
    {
        try
        {
            // recupera o id do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            //var listaTarefas = await tarefacacheService.GetListaTarefasCacheAsync(idUsuario);

            var listaTarefas = await tarefaService.ListaTarefasIdAsync(idUsuario);

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
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar lista tarefas." });
        }
    }

    /// <summary>
    /// Lista tarefas de um usuário com paginação
    /// </summary>
    /// <remarks>
    /// Requer autenticação via JWT.
    ///
    /// Exemplo de requisição:
    ///
    ///     GET /tarefa/lista-paginada?pageNumber=1&amp;pageSize=10
    ///
    /// Exemplo de resposta de sucesso:
    ///
    ///     {
    ///         "items": [
    ///            {
    ///               "id": 1,
    ///               "titulo": "Implementar API",
    ///               "descricao": "Desenvolver endpoints da aplicação",
    ///               "prioridade": "Alta",
    ///               "prazo": 5,
    ///               "status": "Em Progresso",
    ///               "data": "2023-10-25T00:00:00"
    ///            },
    ///            {
    ///               "id": 2,
    ///               "titulo": "Criar documentação",
    ///               "descricao": "Documentar endpoints no Swagger",
    ///               "prioridade": "Média",
    ///               "prazo": 3,
    ///               "status": "Pendente",
    ///               "data": "2023-10-26T00:00:00"
    ///            }
    ///         ],
    ///         "totalCount": 15
    ///     }
    ///
    /// </remarks>
    /// <param name="pageNumber">Número da página (inicia em 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (máximo 50)</param>
    /// <returns>Dados paginados das tarefas do usuário</returns>
    /// <response code="200">Retorna a lista paginada de tarefas</response>
    /// <response code="400">Parâmetros inválidos (valores negativos ou pageSize muito grande)</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Se o usuário não for encontrado</response>
    /// <response code="500">Erro interno no servidor</response>
    //[HttpGet("paginado")]
    [HttpGet("paginado/{status?}")]
    [ProducesResponseType(typeof(PagedResult<TarefaConsultaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<TarefaConsultaDTO>>> ListaTarefasPaginadasAsync(
         [FromRoute] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 15)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new ErrorResponse { message = "Os parâmetros pageNumber e pageSize devem ser maiores que zero." });
            }

            if (pageSize > 50)
            {
                return BadRequest(new ErrorResponse { message = "O tamanho máximo por página é 50 itens." });
            }

            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            var tarefas = await tarefaService.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize, status);

            return Ok(tarefas);
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
            return StatusCode(500, new ErrorResponse
            {
                message = $"Erro interno ao buscar lista de tarefas paginadas. {ex.Message}",
            });
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
    /// <param name="id">ID da tarefa (número inteiro positivo)</param>
    /// <returns>Dados completos da tarefa no formato JSON</returns>
    /// <response code="200">Retorna os dados da tarefa solicitada</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Se nenhuma tarefa for encontrada com o ID fornecido</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IEnumerable<TarefaConsultaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TarefaConsultaDTO>> BuscarTarefaAsync(int id)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            TarefaDTO tarefa = await tarefaService.BuscarTarefaAsync(id, idUsuario);

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
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar lista tarefas." });
        }
    }

    /// <summary>
    /// Busca quantidade de tarefas por status para o usuário autenticado
    /// </summary>
    /// <remarks>
    /// Requer autenticação via JWT.
    ///
    /// Exemplo de requisição:
    ///
    ///     GET /tarefa/status_completo
    ///
    /// Exemplo de resposta de sucesso:
    ///
    ///     {
    ///        "Pendente": 3,
    ///        "EmProgresso": 5,
    ///        "Concluido": 7
    ///     }
    ///
    /// Observações:
    /// - Os valores retornados representam contagens absolutas
    /// - Somente tarefas do usuário autenticado são consideradas
    /// - O ID do usuário é extraído automaticamente do token JWT
    /// </remarks>
    /// <returns>Quantidade de tarefas agrupadas por status no formato JSON</returns>
    /// <response code="200">Retorna as quantidades por status</response>
    /// <response code="401">Token inválido ou ausente</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpGet("status_completo")]
    [ProducesResponseType(typeof(IEnumerable<TarefaQtdStatusDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TarefaQtdStatusDTO>> BuscarTarefaQdtStatusAsync()
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            var tarefa = await tarefaService.ObterQtdStatusEPorcentagemConclusaoAsync(idUsuario);

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
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar quantidade dos status das tarefas." });
        }
    }

    /// <summary>
    /// Obtém as tarefas com prioridade alta que ainda não foram concluídas
    /// </summary>
    /// <remarks>
    /// Requer autenticação via JWT.
    ///
    /// Exemplo de requisição:
    ///
    ///     GET /tarefa/prioridade_alta
    ///
    /// Exemplo de resposta de sucesso:
    ///
    ///     [
    ///         {
    ///             "Titulo": "Revisão de código urgente",
    ///             "Data": "2023-11-15",
    ///             "Prazo": 2
    ///         },
    ///         {
    ///             "Titulo": "Corrigir bug crítico",
    ///             "Data": "2023-11-20",
    ///             "Prazo": 1
    ///         }
    ///     ]
    ///
    /// Observações:
    /// - Retorna uma lista com as tarefas de prioridade alta não concluídas
    /// - Considera apenas tarefas do mês e ano atual
    /// - O ID do usuário é extraído automaticamente do token JWT
    /// </remarks>
    /// <returns>Lista de tarefas de prioridade alta no formato JSON</returns>
    /// <response code="200">Retorna a lista de tarefas de prioridade alta</response>
    /// <response code="401">Token inválido ou ausente</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpGet("prioridade_alta")]
    [ProducesResponseType(typeof(IEnumerable<TarefaPrioridadeAltaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TarefaPrioridadeAltaDTO>>> BuscarTarefasPrioridadeAltaAsync()
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            var tarefasPrioridadeAlta = await tarefaService.BuscarTarefasPrioridadeAltaAsync(idUsuario);

            return Ok(tarefasPrioridadeAlta);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar tarefas com prioridade alta." });
        }
    }
}