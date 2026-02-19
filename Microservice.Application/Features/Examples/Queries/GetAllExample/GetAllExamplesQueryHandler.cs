using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetAllExample
{
    public class GetAllExamplesQueryHandler(
        IExampleRepository appointmentRepository,
        IMapper mapper
        ) : IRequestHandler<GetAllExamplesQuery, IEnumerable<GetAllExamplesDto>>
    {
        public async Task<IEnumerable<GetAllExamplesDto>> Handle(GetAllExamplesQuery request, CancellationToken cancellationToken)
        {
            return mapper.Map<IEnumerable<GetAllExamplesDto>>(await appointmentRepository.GetListAsync(cancellationToken: cancellationToken));

            //otra variante utilizando select
            return [.. await appointmentRepository.GetListAsync(
                select: x => new GetAllExamplesDto
                {
                }
                ,cancellationToken: cancellationToken)];
        }
    }
}
