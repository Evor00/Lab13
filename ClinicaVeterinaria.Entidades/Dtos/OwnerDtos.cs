using System.ComponentModel.DataAnnotations;

namespace ClinicaVeterinaria.Entidades.Dtos;

public class OwnerDto
{
    public int OwnerId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int PetCount { get; set; }
}

public class OwnerCreateDto
{
    [Required, StringLength(120, MinimumLength = 3)]
    public string FullName { get; set; } = null!;
    [Required, StringLength(20)]
    public string Phone { get; set; } = null!;
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;
}

public class OwnerUpdateDto : OwnerCreateDto { }
