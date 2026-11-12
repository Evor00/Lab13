using ClinicaVeterinaria.Entidades.Models;

namespace ClinicaVeterinaria.Datos.Repositories;

public interface IOwnerRepository
{
    Task<List<Owner>> GetAllAsync();
    Task<Owner?> GetByIdAsync(int id);
    Task<Owner> AddAsync(Owner owner);
    Task UpdateAsync(Owner owner);
    Task DeleteAsync(Owner owner);
}
