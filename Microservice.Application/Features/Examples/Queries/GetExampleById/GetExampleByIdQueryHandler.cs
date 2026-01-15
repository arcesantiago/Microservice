using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleById
{
    public class GetExampleByIdQueryHandler(
        IExampleRepository exampleRepository,
        IMapper mapper) : IRequestHandler<GetExampleByIdQuery, GetExampleByIdDto?>
    {
        private readonly IExampleRepository _exampleRepository = exampleRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<GetExampleByIdDto?> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
        {
            var example = await _exampleRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return null;

            return _mapper.Map<GetExampleByIdDto>(example);
        }
    }
}
