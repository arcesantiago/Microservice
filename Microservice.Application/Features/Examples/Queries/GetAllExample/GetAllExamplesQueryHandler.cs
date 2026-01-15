using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetAllExample
{
    public class GetAllExamplesQueryHandler(
        IExampleRepository appointmentRepository,
        IMapper mapper) : IRequestHandler<GetAllExamplesQuery, IEnumerable<GetAllExamplesDto>>
    {
        private readonly IExampleRepository _exampleRepository = appointmentRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<GetAllExamplesDto>> Handle(GetAllExamplesQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<GetAllExamplesDto>>(await _exampleRepository.GetListAsync(cancellationToken: cancellationToken));

            //otra variante utilizando select
            return [.. await _exampleRepository.GetListAsync(
                select: x => new GetAllExamplesDto
                {
                }
                ,cancellationToken: cancellationToken)];
        }
    }
}
