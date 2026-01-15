using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleWithProjection
{
    public record GetExampleWithProjectionQuery(
        int Id
    ) : IRequest<GetExampleWithProjectionDto?>;
}
