using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.CreateExample
{
    public class CreateExampleCommandHandler(
        IExampleRepository exampleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper
        ) : IRequestHandler<CreateExampleCommand, int>
    {
        public async Task<int> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
        {
            var example = mapper.Map<Example>(request);

            await exampleRepository.AddAsync(example, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}