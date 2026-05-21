using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Products.Queries.GetActiveProducts
{
    public record GetActiveProductsQuery : IRequest<Result<IReadOnlyList<GetActiveProductsDto>>>;
}
