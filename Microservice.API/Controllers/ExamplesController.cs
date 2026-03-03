using MediatR;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> CreateExample(
            [FromBody] CreateExampleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return CreatedAtAction(nameof(CreateExample), new { Id = result } , result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GetExampleByPredicateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetExampleByPredicateDto>> GetExampleById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExampleByPredicateQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<GetExamplesPaginatedDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<GetExamplesPaginatedDto>>> GetPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesPaginatedQuery(page, size);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<GetAllExamplesDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetAllExamplesDto>>> GetAllExamples(
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllExamplesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CountExamples(
            CancellationToken cancellationToken = default)
        {
            var query = new CountExamplesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ExistsExample(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new ExistsExampleQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("projection")]
        [ProducesResponseType(typeof(IEnumerable<GetExamplesWithProjectionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetExamplesWithProjectionDto>>> GetExamplesWithProjection(
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesWithProjectionQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}/projection")]
        [ProducesResponseType(typeof(GetExampleWithProjectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetExampleWithProjectionDto>> GetExampleWithProjection(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExampleWithProjectionQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("from-sql")]
        [ProducesResponseType(typeof(IEnumerable<GetExamplesFromSqlDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetExamplesFromSqlDto>>> GetFromSql(
            [FromQuery] string? sql = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetExamplesFromSqlQuery(sql ?? string.Empty);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UpdateExample(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExampleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}/fields")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UpdateExampleFields(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExampleFieldsCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("batch")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> UpdateManyExamples(
            [FromBody] int[] ids,
            CancellationToken cancellationToken)
        {
            var command = new UpdateManyExamplesCommand(ids);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> DeleteExample(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteExampleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("batch")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> DeleteManyExamples(
            [FromBody] int[] ids,
            CancellationToken cancellationToken)
        {
            var command = new DeleteManyExamplesCommand(ids);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("execute-sql")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> ExecuteSql(
            [FromBody] ExecuteSqlCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("execute-stored-procedure")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> ExecuteStoredProcedure(
            [FromBody] ExecuteStoredProcedureCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("execute-sql-with-result")]
        [ProducesResponseType(typeof(IReadOnlyList<ExecuteSqlWithResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExecuteSqlWithResultDto>>> ExecuteSqlWithResult(
            [FromBody] ExecuteSqlWithResultQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("execute-in-transaction")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> ExecuteInTransaction(
            [FromBody] ExecuteInTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}