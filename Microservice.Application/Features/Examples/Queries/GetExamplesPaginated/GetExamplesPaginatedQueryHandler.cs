using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Application.Models;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExamplesPaginated
{
    public class GetExamplesPaginatedQueryHandler(
        IReadRepository<Example> readRepository,
        IMapper mapper) : IRequestHandler<GetExamplesPaginatedQuery, PagedResult<GetExamplesPaginatedDto>>
    {
        public async Task<PagedResult<GetExamplesPaginatedDto>> Handle(GetExamplesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await readRepository.GetListPaginatedAsync(
                request.CurrentPage,
                request.PageSize,
                cancellationToken: cancellationToken);

            var mappedResults = mapper.Map<IEnumerable<GetExamplesPaginatedDto>>(pagedResult.Results);

            return new PagedResult<GetExamplesPaginatedDto>(
                mappedResults,
                pagedResult.RowsCount,
                pagedResult.CurrentPage,
                pagedResult.PageSize);
        }
    }
}
