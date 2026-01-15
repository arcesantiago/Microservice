using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetAllExample
{
    public record GetAllExamplesQuery : IRequest<IEnumerable<GetAllExamplesDto>>;
}
