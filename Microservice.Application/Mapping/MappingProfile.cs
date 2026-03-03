using AutoMapper;
using Microservice.Application.DTOs;
using Microservice.Application.Features.Examples.Commands.CreateExample;
using Microservice.Domain.Entities;

namespace Microservice.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreateExampleCommand, Example>()
                .ConstructUsing(src => new Example(src.Id));

            CreateMap<Example, GetExampleByIdDto>();
            CreateMap<Example, GetExampleByPredicateDto>();
            CreateMap<Example, GetExamplesFromSqlDto>();
            CreateMap<Example, GetExamplesPaginatedDto>();
            CreateMap<Example, GetExamplesWithProjectionDto>();
            CreateMap<Example, GetExampleWithProjectionDto>();
            CreateMap<Example, ExecuteSqlWithResultDto>();
        }
    }
}
