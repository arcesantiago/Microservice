using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Products.Queries.GetActiveProducts;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(Result<IReadOnlyList<GetActiveProductsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveProducts(CancellationToken cancellationToken)
        {
            var query = new GetActiveProductsQuery();
            return Ok(await mediator.Send(query, cancellationToken));
        }
    }
}
