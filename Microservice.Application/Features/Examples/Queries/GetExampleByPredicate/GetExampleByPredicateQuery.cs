using MediatR;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleByPredicate
{
    public record GetExampleByPredicateQuery(
        int Id
    ) : IRequest<GetExampleByPredicateDto?>;
}
