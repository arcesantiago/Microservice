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
        }
    }
}
