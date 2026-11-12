using ClinicaVeterinaria.Entidades.Models;

namespace ClinicaVeterinaria.Datos.Repositories;

public interface IPetRepository
{
    Task<List<Pet>> GetAllAsync(string? species, int? ownerId);
    Task<List<Pet>> GetByOwnerIdAsync(int ownerId);
    Task<Pet?> GetByIdAsync(int id);
    Task<bool> ExistsWithNameAsync(int ownerId, string name, int? exceptPetId = null);
    Task<Pet> AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Pet pet);
}
