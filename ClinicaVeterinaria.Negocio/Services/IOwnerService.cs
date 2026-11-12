using ClinicaVeterinaria.Entidades.Dtos;

namespace ClinicaVeterinaria.Negocio.Services;

public interface IOwnerService
{
    Task<List<OwnerDto>> GetAllAsync();
    Task<OwnerDto?> GetByIdAsync(int id);
    Task<OwnerDto> CreateAsync(OwnerCreateDto dto);
    Task<bool> UpdateAsync(int id, OwnerUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
