using ClinicaVeterinaria.Entidades.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicaVeterinaria.Datos.Context;

// Contexto preparado para el flujo database-first indicado en la guía.
public partial class ClinicaVeterinariaContext : DbContext
{
    public ClinicaVeterinariaContext(DbContextOptions<ClinicaVeterinariaContext> options) : base(options) { }
    public virtual DbSet<Owner> Owners => Set<Owner>();
    public virtual DbSet<Pet> Pets => Set<Pet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.ToTable("Owners");
            entity.HasKey(e => e.OwnerId);
            entity.Property(e => e.FullName).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("Pets");
            entity.HasKey(e => e.PetId);
            entity.Property(e => e.Name).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Species).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Breed).HasMaxLength(60).IsRequired();
            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.HasIndex(e => new { e.OwnerId, e.Name }).IsUnique();
            entity.HasOne(e => e.Owner).WithMany(o => o.Pets).HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Pets_Owners");
        });
    }
}
