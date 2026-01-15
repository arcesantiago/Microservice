using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesWithProjection
{
    public record GetExamplesWithProjectionQuery : IRequest<IEnumerable<GetExamplesWithProjectionDto>>;
}
