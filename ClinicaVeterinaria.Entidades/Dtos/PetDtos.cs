using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Entidades.Dtos;

public class PetDto
{
    public int PetId { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Species { get; set; } = null!;
    public string Breed { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}

public class PetCreateDto
{
    [Range(1, int.MaxValue)]
    public int OwnerId { get; set; }
    [Required, StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = null!;
    [Required, StringLength(30)]
    public string Species { get; set; } = null!;
    [Required, StringLength(60)]
    public string Breed { get; set; } = null!;
    [Range(typeof(DateTime), "2000-01-01", "2100-01-01")]
    public DateTime BirthDate { get; set; }
}

public class PetUpdateDto
{
    [Required, StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = null!;
    [Required, StringLength(30)]
    public string Species { get; set; } = null!;
    [Required, StringLength(60)]
    public string Breed { get; set; } = null!;
    [Range(typeof(DateTime), "2000-01-01", "2100-01-01")]
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}
