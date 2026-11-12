using AutoMapper;
using ClinicaVeterinaria.Datos.Repositories;
using ClinicaVeterinaria.Entidades.Dtos;
using ClinicaVeterinaria.Entidades.Models;
using ClinicaVeterinaria.Negocio.Exceptions;

namespace ClinicaVeterinaria.Negocio.Services;

public class PetService(IPetRepository pets, IOwnerRepository owners, IMapper mapper) : IPetService
{
    public async Task<List<PetDto>> GetAllAsync(string? species, int? ownerId) => mapper.Map<List<PetDto>>(await pets.GetAllAsync(species, ownerId));
    public async Task<List<PetDto>?> GetByOwnerIdAsync(int ownerId)
    {
        if (await owners.GetByIdAsync(ownerId) is null) return null;
        return mapper.Map<List<PetDto>>(await pets.GetByOwnerIdAsync(ownerId));
    }
    public async Task<PetDto?> GetByIdAsync(int id) { var pet = await pets.GetByIdAsync(id); return pet is null ? null : mapper.Map<PetDto>(pet); }
    public async Task<PetDto> CreateAsync(PetCreateDto dto)
    {
        if (await owners.GetByIdAsync(dto.OwnerId) is null) throw new ReglaNegocioException("El propietario indicado no existe.");
        if (await pets.ExistsWithNameAsync(dto.OwnerId, dto.Name)) throw new ReglaNegocioException("Ya existe una mascota con ese nombre para el propietario.");
        var pet = mapper.Map<Pet>(dto);
        await pets.AddAsync(pet);
        return mapper.Map<PetDto>((await pets.GetByIdAsync(pet.PetId))!);
    }
    public async Task<bool> UpdateAsync(int id, PetUpdateDto dto)
    {
        var pet = await pets.GetByIdAsync(id);
        if (pet is null) return false;
        if (await pets.ExistsWithNameAsync(pet.OwnerId, dto.Name, id)) throw new ReglaNegocioException("Ya existe una mascota con ese nombre para el propietario.");
        mapper.Map(dto, pet);
        await pets.UpdateAsync(pet);
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var pet = await pets.GetByIdAsync(id);
        if (pet is null) return false;
        await pets.DeleteAsync(pet);
        return true;
    }
}
