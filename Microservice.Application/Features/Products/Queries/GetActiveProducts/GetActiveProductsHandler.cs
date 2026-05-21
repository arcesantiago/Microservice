using AutoMapper;
using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence.Dapper;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Products.Queries.GetActiveProducts
{
    public sealed class GetActiveProductsHandler(
        IDapperReadRepository<Product> repo,
        IMapper mapper
        )
        : IRequestHandler<GetActiveProductsQuery, Result<IReadOnlyList<GetActiveProductsDto>>>
    {
        public async Task<Result<IReadOnlyList<GetActiveProductsDto>>> Handle(
            GetActiveProductsQuery request, CancellationToken ct)
        {
            var products = await repo.GetListAsync(
                new QueryParameters<Product>
                {
                    Filters = new() { ["is_active"] = true },
                    OrderByColumn = "name",
                    Columns = ["id", "name", "price"]
                }, ct);

            var result = mapper.Map<IReadOnlyList<GetActiveProductsDto>>(products);

            return Result<IReadOnlyList<GetActiveProductsDto>>.Success(result);
        }
    }
}
