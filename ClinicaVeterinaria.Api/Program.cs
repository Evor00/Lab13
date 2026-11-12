using ClinicaVeterinaria.Datos.Context;
using ClinicaVeterinaria.Datos.Repositories;
using ClinicaVeterinaria.Entidades.Models;
using ClinicaVeterinaria.Negocio.Mapping;
using ClinicaVeterinaria.Negocio.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
    builder.Services.AddDbContext<ClinicaVeterinariaContext>(o => o.UseInMemoryDatabase("ClinicaVeterinaria"));
else
    builder.Services.AddDbContext<ClinicaVeterinariaContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("ClinicaVeterinaria")));
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IPetService, PetService>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicaVeterinariaContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Owners.AnyAsync())
    {
        var owner = new Owner { FullName = "Lucia Torres", Phone = "999123456", Email = "lucia.torres@example.com" };
        db.Owners.Add(owner);
        db.Pets.AddRange(new Pet { Owner = owner, Name = "Milo", Species = "Perro", Breed = "Labrador", BirthDate = new DateTime(2021, 5, 10), IsActive = true }, new Pet { Owner = owner, Name = "Nina", Species = "Gato", Breed = "Siamés", BirthDate = new DateTime(2022, 8, 22), IsActive = true });
        await db.SaveChangesAsync();
    }
}
await app.RunAsync();
