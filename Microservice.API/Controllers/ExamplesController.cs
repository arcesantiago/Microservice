using MediatR;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Examples.Commands.CreateExample;
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
            [FromBody] CreateExampleDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreateExampleCommand(
                dto.Id);

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateExample), new { id = result }, result);
        }
    }
}