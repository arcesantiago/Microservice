using MediatR;
using Microservice.Application.DTOs;
using Microservice.Application.Models;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesPaginated
{
    public record GetExamplesPaginatedQuery(
        int CurrentPage,
        int PageSize
    ) : IRequest<PagedResult<GetExamplesPaginatedDto>>;
}
