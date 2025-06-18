using AutoMapper;
using KindergartenAPI.DTOs.Ninos;
using KindergartenAPI.DTOs.Personas;
using KindergartenAPI.Models;

namespace KindergartenAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Nino mappings
            CreateMap<NinoCreateDto, Nino>();
            CreateMap<NinoUpdateDto, Nino>();
            CreateMap<Nino, NinoReadDto>();
            CreateMap<Nino, NinoWithPersonaReadDto>()
                .ForMember(dest => dest.Pagador, opt => opt.MapFrom(src => src.CedulaPagadorNavigation));

            // Persona mappings
            CreateMap<PersonaCreateDto, Persona>();
            CreateMap<PersonaUpdateDto, Persona>();
            CreateMap<Persona, PersonaReadDto>();
        }
    }
}