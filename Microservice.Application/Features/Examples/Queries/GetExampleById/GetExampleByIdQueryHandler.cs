using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExampleById
{
    public class GetExampleByIdQueryHandler(
        IReadRepository<Example> readRepository,
        IMapper mapper
        ) : IRequestHandler<GetExampleByIdQuery, GetExampleByIdDto?>
    {
        public async Task<GetExampleByIdDto?> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
        {
            var example = await readRepository.FindAsync(request.Id, cancellationToken);

            if (example == null)
                return null;

            return mapper.Map<GetExampleByIdDto>(example);
        }
    }
}
