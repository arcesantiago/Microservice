using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetAllExample
{
    public class GetAllExamplesQueryHandler(
        IReadRepository<Example> readRepository,
        IQueryRepository<Example> queryRepository,
        IMapper mapper
        ) : IRequestHandler<GetAllExamplesQuery, IEnumerable<GetAllExamplesDto>>
    {
        public async Task<IEnumerable<GetAllExamplesDto>> Handle(GetAllExamplesQuery request, CancellationToken cancellationToken)
        {
            return mapper.Map<IEnumerable<GetAllExamplesDto>>(await readRepository.GetListAsync(cancellationToken: cancellationToken));

            //otra variante utilizando select
            //return [.. await queryRepository.GetListAsync(
            //    select: x => new GetAllExamplesDto
            //    {
            //    }
            //    ,cancellationToken: cancellationToken)];
        }
    }
}
