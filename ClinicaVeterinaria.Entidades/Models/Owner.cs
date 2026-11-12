namespace ClinicaVeterinaria.Entidades.Models;

public class Owner
{
    public int OwnerId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
