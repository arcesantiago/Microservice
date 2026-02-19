using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Application.DTOs;

namespace Microservice.Application.Features.Examples.Queries.GetExampleById
{
    public class GetExampleByIdQueryHandler(
        IExampleRepository exampleRepository,
        IMapper mapper
        ) : IRequestHandler<GetExampleByIdQuery, GetExampleByIdDto?>
    {
        public async Task<GetExampleByIdDto?> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
        {
            var example = await exampleRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return null;

            return mapper.Map<GetExampleByIdDto>(example);
        }
    }
}
