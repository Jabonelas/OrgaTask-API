﻿using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.Controllers.Tarefa;

[ApiController]
[Route("api/tarefas")]
[Authorize]
public class TarefaController : ControllerBase
{
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

    public enum PrioridadeTarefa
    {
        Alta,
        Média,
        Baixa
    }

    public enum StatusTarefa
    {
        Pendente,
        Em_Progresso,
        Concluído,
        Todas
    }


    /// <summary>
    /// Registra uma nova tarefa no vinculada ao usuário atualmente autenticado.
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
    ///        "prazo": "5"
    ///     }
    ///
    /// Exemplo de resposta de sucesso:
    ///
    ///     "message": "Tarefa cadastrada com sucesso!"
    ///
    /// </remarks>
    /// <param name="_idUsuario">ID do usuário (número inteiro positivo) que receberá a tarefa</param>
    /// <param name="teste">DTO com os dados necessários para cadastro da tarefa</param>
    /// <returns>Mensagem de confirmação do cadastro</returns>
    /// <response code="201">Retorna mensagem de sucesso ao cadastrar a tarefa</response>
    /// <response code="400">Se os dados da tarefa forem inválidos</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(Response), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CadastrarTarefaAsync(TarefaCadastrarDTO _dadosTarefaCadastro, [FromQuery][Required] PrioridadeTarefa? prioridade = null, [FromQuery][Required] StatusTarefa? status = null)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            if (!prioridade.HasValue)
                return BadRequest(new Response { message = "Prioridade inválida. Valores aceitos: " + string.Join(", ", Enum.GetNames(typeof(PrioridadeTarefa))) });

            if (!status.HasValue)
                return BadRequest(new Response { message = "Status inválido. Valores aceitos: " + string.Join(", ", Enum.GetNames(typeof(StatusTarefa))) });

            await tarefaService.CadastrarTarefaAsync(idUsuario, _dadosTarefaCadastro, prioridade.ToString(), status.ToString());

            //await tarefacacheService.InvalidarCache(idUsuario);

            return Created("", new Response { message = "Tarefa cadastrada com sucesso!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Response { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao cadastrar tarefa." });
        }
    }

    /// <summary>
    /// Modifica os dados de uma tarefa existente. Apenas o usuário dono da tarefa pode realizar a alteração. Requer autenticação válida.
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
    ///        "prazo": 2
    ///     }
    ///
    /// Exemplo de resposta de sucesso:
    ///
    ///     "message": "Tarefa alterada com sucesso!"
    ///
    /// </remarks>
    /// <param name="_dadosTarefaCadastro">DTO com os dados necessários para alteração da tarefa</param>
    /// <returns>Mensagem de confirmação da alteração</returns>
    /// <response code="200">Retorna mensagem de sucesso ao alterar a tarefa</response>
    /// <response code="400">Se os dados da tarefa forem inválidos</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="404">Se a tarefa não for encontrada</response>
    /// <response code="422">Se a operação não puder ser concluída devido a regras de negócio</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpPut]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse400), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AlterarTarefaAsync(TarefaAlterarDTO _dadosTarefaCadastro, [FromQuery][Required] PrioridadeTarefa? prioridade = null, [FromQuery][Required] StatusTarefa? status = null)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            if (!prioridade.HasValue)
                return BadRequest(new Response { message = "Prioridade inválida. Valores aceitos: " + string.Join(", ", Enum.GetNames(typeof(PrioridadeTarefa))) });

            if (!status.HasValue)
                return BadRequest(new Response { message = "Status inválido. Valores aceitos: " + string.Join(", ", Enum.GetNames(typeof(StatusTarefa))) });


            await tarefaService.AlterarTarefaAsync(idUsuario, _dadosTarefaCadastro, prioridade.ToString(), status.ToString());

            //await tarefacacheService.InvalidarCache(idUsuario);

            return Ok(new Response { message = "Tarefa alterada com sucesso!" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Response { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(422, new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao alterar tarefa." });
        }
    }

    /// <summary>
    /// Remove permanentemente uma tarefa específica pelo ID. Requer autenticação válida.
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
    /// <response code="200">Retorna mensagem de sucesso ao deletar a tarefa</response>
    /// <response code="401">Acesso não autorizado (token inválido ou ausente)</response>
    /// <response code="422">Se a operação não puder ser concluída devido a regras de negócio</response>
    /// <response code="500">Erro interno no servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeletarTarefaAsync(int id)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            await tarefaService.DeletarTarefaAsync(id, idUsuario);

            //await tarefacacheService.InvalidarCache(idUsuario);

            return Ok(new Response { message = "Tarefa deletada com sucesso!" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(422, new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao deletar tarefa." });
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
    //[HttpGet]
    //[ProducesResponseType(typeof(IEnumerable<TarefaConsultaDTO>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    //public async Task<ActionResult<IEnumerable<TarefaConsultaDTO>>> ListaTarefas()
    //{
    //    try
    //    {
    //        // recupera o id do usuário do token
    //        _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

    //        //var listaTarefas = await tarefacacheService.GetListaTarefasCacheAsync(idUsuario);

    //        var listaTarefas = await tarefaService.ListaTarefasIdAsync(idUsuario);

    //        return Ok(listaTarefas);
    //    }
    //    catch (UnauthorizedAccessException ex)
    //    {
    //        return Unauthorized(new ErrorResponse { message = ex.Message });
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(new ErrorResponse { message = ex.Message });
    //    }
    //    catch (Exception)
    //    {
    //        return StatusCode(500, new ErrorResponse { message = "Erro interno ao buscar lista tarefas." });
    //    }
    //}

    /// <summary>
    ///
    /// Lista todas as tarefas associadas ao usuário atualmente autenticado, com paginação.
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
    /// <response code="500">Erro interno no servidor</response>
    //[HttpGet("paginado")]
    [HttpGet("paginado/{status?}")]
    [ProducesResponseType(typeof(PagedResult<TarefaConsultaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<TarefaConsultaDTO>>> ListaTarefasPaginadasAsync(
    [FromRoute][Required] StatusTarefa? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 15)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new Response { message = "Os parâmetros pageNumber e pageSize devem ser maiores que zero." });
            }

            if (pageSize > 50)
            {
                return BadRequest(new Response { message = "O tamanho máximo por página é 50 itens." });
            }

            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            var tarefas = await tarefaService.ListaTarefasPaginadasAsync(idUsuario, pageNumber, pageSize, status.ToString());

            return Ok(tarefas);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Response { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response
            {
                message = $"Erro interno ao buscar lista de tarefas paginadas. {ex.Message}",
            });
        }
    }



    /// <summary>
    /// Recupera os detalhes completos de uma tarefa específica usando seu ID. Acesso permitido apenas para o usuário dono da tarefa.
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
    ///        "id": 1,
    ///        "titulo": "Implementar API",
    ///        "descricao": "Desenvolver endpoints da aplicação",
    ///        "prioridade": "Alta",
    ///        "prazo": 7,
    ///        "status": "Em andamento",
    ///        "dataCriacao": "7/3/2025 3:39:38 PM",
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
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TarefaConsultaDTO>> BuscarTarefaAsync(int id)
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            TarefaConsultaDTO tarefa = await tarefaService.BuscarTarefaAsync(id, idUsuario);

            return Ok(tarefa);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao buscar tarefas." });
        }
    }

    /// <summary>
    /// Obtém estatísticas de quantidade de tarefas agrupadas por status (ex: pendentes, concluídas), exclusivas para o usuário autenticado.
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
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TarefaQtdStatusDTO>> BuscarQtdStatusEPorcentagemConclusaoAsync()
    {
        try
        {
            // Recupera o ID do usuário do token
            _ = int.TryParse(User.FindFirst("idUsuario")?.Value, out var idUsuario);

            var tarefa = await tarefaService.BuscarQtdStatusEPorcentagemConclusaoAsync(idUsuario);

            return Ok(tarefa);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao buscar quantidade dos status das tarefas." });
        }
    }

    /// <summary>
    /// Obtém a lista de tarefas não concluídas com prioridade alta, exclusivas para o usuário atualmente autenticado.
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
    [ProducesResponseType(typeof(Response), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
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
            return Unauthorized(new Response { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Response { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new Response { message = "Erro interno ao buscar tarefas com prioridade alta." });
        }
    }
}