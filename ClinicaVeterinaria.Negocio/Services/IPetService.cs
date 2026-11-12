using ClinicaVeterinaria.Entidades.Dtos;

namespace ClinicaVeterinaria.Negocio.Services;

public interface IPetService
{
    Task<List<PetDto>> GetAllAsync(string? species, int? ownerId);
    Task<List<PetDto>?> GetByOwnerIdAsync(int ownerId);
    Task<PetDto?> GetByIdAsync(int id);
    Task<PetDto> CreateAsync(PetCreateDto dto);
    Task<bool> UpdateAsync(int id, PetUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
