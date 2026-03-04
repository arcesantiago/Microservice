using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Examples.Commands.CreateExample;
using Microservice.Application.Features.Examples.Commands.DeleteExample;
using Microservice.Application.Features.Examples.Commands.DeleteManyExamples;
using Microservice.Application.Features.Examples.Commands.ExecuteInTransaction;
using Microservice.Application.Features.Examples.Commands.ExecuteSql;
using Microservice.Application.Features.Examples.Commands.ExecuteStoredProcedure;
using Microservice.Application.Features.Examples.Commands.UpdateExample;
using Microservice.Application.Features.Examples.Commands.UpdateExampleFields;
using Microservice.Application.Features.Examples.Commands.UpdateManyExamples;
using Microservice.Application.Features.Examples.Queries.CountExamples;
using Microservice.Application.Features.Examples.Queries.ExecuteSqlWithResult;
using Microservice.Application.Features.Examples.Queries.ExistsExample;
using Microservice.Application.Features.Examples.Queries.GetAllExample;
using Microservice.Application.Features.Examples.Queries.GetExampleByPredicate;
using Microservice.Application.Features.Examples.Queries.GetExamplesFromSql;
using Microservice.Application.Features.Examples.Queries.GetExamplesPaginated;
using Microservice.Application.Features.Examples.Queries.GetExamplesWithProjection;
using Microservice.Application.Features.Examples.Queries.GetExampleWithProjection;
using Microservice.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamplesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<int>>> CreateExample(
            [FromBody] CreateExampleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetExampleById), new { id = result.Data }, result)
                : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Result<GetExampleByPredicateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<GetExampleByPredicateDto>>> GetExampleById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExampleByPredicateQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : NotFound(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Result<PagedResult<GetExamplesPaginatedDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<PagedResult<GetExamplesPaginatedDto>>>> GetPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesPaginatedQuery(page, size);
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(Result<IEnumerable<GetAllExamplesDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<IEnumerable<GetAllExamplesDto>>>> GetAllExamples(
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllExamplesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> CountExamples(
            CancellationToken cancellationToken = default)
        {
            var query = new CountExamplesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<bool>>> ExistsExample(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new ExistsExampleQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("projection")]
        [ProducesResponseType(typeof(Result<IEnumerable<GetExamplesWithProjectionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<IEnumerable<GetExamplesWithProjectionDto>>>> GetExamplesWithProjection(
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesWithProjectionQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("{id:int}/projection")]
        [ProducesResponseType(typeof(Result<GetExampleWithProjectionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<GetExampleWithProjectionDto>>> GetExampleWithProjection(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExampleWithProjectionQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : NotFound(result);
        }

        [HttpGet("from-sql")]
        [ProducesResponseType(typeof(Result<IEnumerable<GetExamplesFromSqlDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<IEnumerable<GetExamplesFromSqlDto>>>> GetFromSql(
            [FromQuery] string? sql = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesFromSqlQuery(sql ?? string.Empty);
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<int>>> UpdateExample(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExampleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : NotFound(result);
        }

        [HttpPut("{id:int}/fields")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<int>>> UpdateExampleFields(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExampleFieldsCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : NotFound(result);
        }

        [HttpPut("batch")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> UpdateManyExamples(
            [FromBody] int[] ids,
            CancellationToken cancellationToken)
        {
            var command = new UpdateManyExamplesCommand(ids);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<int>>> DeleteExample(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteExampleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : NotFound(result);
        }

        [HttpDelete("batch")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> DeleteManyExamples(
            [FromBody] int[] ids,
            CancellationToken cancellationToken)
        {
            var command = new DeleteManyExamplesCommand(ids);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("execute-sql")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> ExecuteSql(
            [FromBody] ExecuteSqlCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("execute-stored-procedure")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> ExecuteStoredProcedure(
            [FromBody] ExecuteStoredProcedureCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("execute-sql-with-result")]
        [ProducesResponseType(typeof(Result<IReadOnlyList<ExecuteSqlWithResultDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<IReadOnlyList<ExecuteSqlWithResultDto>>>> ExecuteSqlWithResult(
            [FromBody] ExecuteSqlWithResultQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("execute-in-transaction")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<int>>> ExecuteInTransaction(
            [FromBody] ExecuteInTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : BadRequest(result);
        }
    }
}