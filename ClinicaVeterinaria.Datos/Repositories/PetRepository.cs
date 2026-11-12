using ClinicaVeterinaria.Datos.Context;
using ClinicaVeterinaria.Entidades.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Datos.Repositories;

public class PetRepository(ClinicaVeterinariaContext context) : IPetRepository
{
    public Task<List<Pet>> GetAllAsync(string? species, int? ownerId)
    {
        IQueryable<Pet> query = context.Pets.Include(x => x.Owner);
        if (!string.IsNullOrWhiteSpace(species)) query = query.Where(x => x.Species == species);
        if (ownerId.HasValue) query = query.Where(x => x.OwnerId == ownerId.Value);
        return query.OrderBy(x => x.Name).ToListAsync();
    }
    public Task<List<Pet>> GetByOwnerIdAsync(int ownerId) => context.Pets.Include(x => x.Owner).Where(x => x.OwnerId == ownerId).OrderBy(x => x.Name).ToListAsync();
    public Task<Pet?> GetByIdAsync(int id) => context.Pets.Include(x => x.Owner).FirstOrDefaultAsync(x => x.PetId == id);
    public Task<bool> ExistsWithNameAsync(int ownerId, string name, int? exceptPetId = null) => context.Pets.AnyAsync(x => x.OwnerId == ownerId && x.Name == name && (!exceptPetId.HasValue || x.PetId != exceptPetId));
    public async Task<Pet> AddAsync(Pet pet) { context.Pets.Add(pet); await context.SaveChangesAsync(); return pet; }
    public async Task UpdateAsync(Pet pet) { context.Pets.Update(pet); await context.SaveChangesAsync(); }
    public async Task DeleteAsync(Pet pet) { context.Pets.Remove(pet); await context.SaveChangesAsync(); }
}
