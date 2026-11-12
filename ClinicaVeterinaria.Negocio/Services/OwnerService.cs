using AutoMapper;
using ClinicaVeterinaria.Datos.Repositories;
using ClinicaVeterinaria.Entidades.Dtos;
using ClinicaVeterinaria.Entidades.Models;
using ClinicaVeterinaria.Negocio.Exceptions;

namespace ClinicaVeterinaria.Negocio.Services;

public class OwnerService(IOwnerRepository owners, IMapper mapper) : IOwnerService
{
    public async Task<List<OwnerDto>> GetAllAsync() => mapper.Map<List<OwnerDto>>(await owners.GetAllAsync());
    public async Task<OwnerDto?> GetByIdAsync(int id) { var owner = await owners.GetByIdAsync(id); return owner is null ? null : mapper.Map<OwnerDto>(owner); }
    public async Task<OwnerDto> CreateAsync(OwnerCreateDto dto)
    {
        var owner = mapper.Map<Owner>(dto);
        await owners.AddAsync(owner);
        return mapper.Map<OwnerDto>(owner);
    }
    public async Task<bool> UpdateAsync(int id, OwnerUpdateDto dto)
    {
        var owner = await owners.GetByIdAsync(id);
        if (owner is null) return false;
        mapper.Map(dto, owner);
        await owners.UpdateAsync(owner);
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var owner = await owners.GetByIdAsync(id);
        if (owner is null) return false;
        if (owner.Pets.Any(p => p.IsActive)) throw new ReglaNegocioException("No se puede eliminar un propietario con mascotas activas.");
        await owners.DeleteAsync(owner);
        return true;
    }
}
