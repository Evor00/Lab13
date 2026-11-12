namespace ClinicaVeterinaria.Entidades.Models;

public class Pet
{
    public int PetId { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string Species { get; set; } = null!;
    public string Breed { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
    public virtual Owner Owner { get; set; } = null!;
}
