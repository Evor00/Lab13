using ClinicaVeterinaria.Entidades.Dtos;
using ClinicaVeterinaria.Negocio.Exceptions;
using ClinicaVeterinaria.Negocio.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVeterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(IPetService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<List<PetDto>>> GetAll([FromQuery] string? species, [FromQuery] int? ownerId) => Ok(await service.GetAllAsync(species, ownerId));
    [HttpGet("{id:int}")] public async Task<ActionResult<PetDto>> GetById(int id) => (await service.GetByIdAsync(id)) is { } pet ? Ok(pet) : NotFound();
    [HttpGet("/api/owners/{ownerId:int}/pets")] public async Task<ActionResult<List<PetDto>>> GetByOwner(int ownerId) => (await service.GetByOwnerIdAsync(ownerId)) is { } pets ? Ok(pets) : NotFound();
    [HttpPost] public async Task<ActionResult<PetDto>> Create(PetCreateDto dto) { try { var pet = await service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = pet.PetId }, pet); } catch (ReglaNegocioException ex) { return Conflict(new { message = ex.Message }); } }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, PetUpdateDto dto) { try { return await service.UpdateAsync(id, dto) ? NoContent() : NotFound(); } catch (ReglaNegocioException ex) { return Conflict(new { message = ex.Message }); } }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) => await service.DeleteAsync(id) ? NoContent() : NotFound();
}
