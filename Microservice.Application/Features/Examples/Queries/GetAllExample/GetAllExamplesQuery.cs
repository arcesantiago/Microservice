using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetAllExample
{
    public record GetAllExamplesQuery : IRequest<Result<IEnumerable<GetAllExamplesDto>>>;
}
