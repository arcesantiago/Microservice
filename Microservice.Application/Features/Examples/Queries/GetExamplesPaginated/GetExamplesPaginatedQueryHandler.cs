using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Examples.Queries.GetExamplesPaginated;
using Microservice.Application.Models;

namespace ModularMonolith.Modules.Examples.Core.Features.Examples.Queries.GetExamplesPaginated
{
    public class GetExamplesPaginatedQueryHandler(
        IExampleRepository exampleRepository,
        IMapper mapper) : IRequestHandler<GetExamplesPaginatedQuery, PagedResult<GetExamplesPaginatedDto>>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<GetExamplesPaginatedDto>> Handle(GetExamplesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _exampleRepository.GetListPaginatedAsync(
                request.CurrentPage,
                request.PageSize,
                cancellationToken: cancellationToken);

            var mappedResults = _mapper.Map<IEnumerable<GetExamplesPaginatedDto>>(pagedResult.Results);

            return new PagedResult<GetExamplesPaginatedDto>(
                mappedResults,
                pagedResult.RowsCount,
                pagedResult.CurrentPage,
                pagedResult.PageSize);
        }
    }
}
