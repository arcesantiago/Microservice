using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleById
{
    public record GetExampleByIdQuery(
        int Id
    ) : IRequest<GetExampleByIdDto?>;
}
