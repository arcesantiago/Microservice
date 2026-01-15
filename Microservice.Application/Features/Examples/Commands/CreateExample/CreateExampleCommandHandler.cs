using AutoMapper;
using MediatR;
using Microservice.Application.Contracts.Persistence;
using Microservice.Domain.Entities;

namespace Microservice.Application.Features.Examples.Commands.CreateExample
{
    public class CreateExampleCommandHandler(
        IExampleUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateExampleCommand, int>
    {
        private readonly IExampleUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<int> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
        {
            var example = _mapper.Map<Example>(request);

            await _unitOfWork.Examples.AddAsync(example, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return example.Id;
        }
    }
}