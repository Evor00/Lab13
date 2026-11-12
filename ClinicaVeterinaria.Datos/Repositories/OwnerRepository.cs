using ClinicaVeterinaria.Datos.Context;
using ClinicaVeterinaria.Entidades.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Datos.Repositories;

public class OwnerRepository(ClinicaVeterinariaContext context) : IOwnerRepository
{
    public Task<List<Owner>> GetAllAsync() => context.Owners.Include(x => x.Pets).OrderBy(x => x.FullName).ToListAsync();
    public Task<Owner?> GetByIdAsync(int id) => context.Owners.Include(x => x.Pets).FirstOrDefaultAsync(x => x.OwnerId == id);
    public async Task<Owner> AddAsync(Owner owner) { context.Owners.Add(owner); await context.SaveChangesAsync(); return owner; }
    public async Task UpdateAsync(Owner owner) { context.Owners.Update(owner); await context.SaveChangesAsync(); }
    public async Task DeleteAsync(Owner owner) { context.Owners.Remove(owner); await context.SaveChangesAsync(); }
}
