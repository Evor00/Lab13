using AutoMapper;
using ClinicaVeterinaria.Entidades.Dtos;
using ClinicaVeterinaria.Entidades.Models;

namespace ClinicaVeterinaria.Negocio.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Owner, OwnerDto>()
            .ForMember(d => d.PetCount, o => o.MapFrom(s => s.Pets.Count));
        CreateMap<OwnerCreateDto, Owner>();
        CreateMap<OwnerUpdateDto, Owner>();
        CreateMap<Pet, PetDto>()
            .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner.FullName));
        CreateMap<PetCreateDto, Pet>()
            .ForMember(d => d.IsActive, o => o.MapFrom(_ => true));
        CreateMap<PetUpdateDto, Pet>()
            .ForMember(d => d.OwnerId, o => o.Ignore());
    }
}
