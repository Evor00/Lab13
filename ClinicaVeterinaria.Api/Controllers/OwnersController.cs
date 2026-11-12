using ClinicaVeterinaria.Entidades.Dtos;
using ClinicaVeterinaria.Negocio.Exceptions;
using ClinicaVeterinaria.Negocio.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaVeterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnersController(IOwnerService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<List<OwnerDto>>> GetAll() => Ok(await service.GetAllAsync());
    [HttpGet("{id:int}")] public async Task<ActionResult<OwnerDto>> GetById(int id) => (await service.GetByIdAsync(id)) is { } owner ? Ok(owner) : NotFound();
    [HttpPost] public async Task<ActionResult<OwnerDto>> Create(OwnerCreateDto dto) { var owner = await service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = owner.OwnerId }, owner); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, OwnerUpdateDto dto) => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { try { return await service.DeleteAsync(id) ? NoContent() : NotFound(); } catch (ReglaNegocioException ex) { return Conflict(new { message = ex.Message }); } }
}
