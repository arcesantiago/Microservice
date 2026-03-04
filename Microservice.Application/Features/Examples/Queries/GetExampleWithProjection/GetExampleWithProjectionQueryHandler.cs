using MediatR;
using Microservice.Application.Common.Results;
using Microservice.Application.Contracts.Persistence.EF;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Queries.GetExampleWithProjection
{
    public class GetExampleWithProjectionQueryHandler(
        IQueryRepository<Example> queryRepository
        ) : IRequestHandler<GetExampleWithProjectionQuery, Result<GetExampleWithProjectionDto>>
    {
        public async Task<Result<GetExampleWithProjectionDto>> Handle(GetExampleWithProjectionQuery request, CancellationToken cancellationToken)
        {
            var data = await queryRepository.GetEntityAsync(x => new GetExampleWithProjectionDto { Id = x.Id}, x => x.Id == request.Id, cancellationToken);
            
            if (data == null)
                return Result<GetExampleWithProjectionDto>.Failure("Ejemplo no encontrado");
            
            return Result<GetExampleWithProjectionDto>.Success(data);
        }
    }
}
