using AutoMapper;
using Microservice.Application.DTOs;
using Microservice.Domain.Entities;

namespace Microservice.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreateExampleDto, Example>()
                .ConstructUsing(src => new Example(src.Id));
        }
    }
}
