
using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public BodegaController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/Bodega
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bodega>>> GetBodegas()
        {
            return Ok(await _context.Bodegas
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/Bodega/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Bodega>> GetBodega(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return NotFound("Bodega no encontrada");

            return Ok(bodega);
        }

        // POST: api/Bodega
        [HttpPost]
        public async Task<ActionResult<Bodega>> CrearBodega(Bodega bodega)
        {
            bodega.IdBodega = 0;

            _context.Bodegas.Add(bodega);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBodega),
                new { id = bodega.IdBodega },
                bodega);
        }

        // PUT: api/Bodega/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarBodega(
            int id, Bodega bodega)
        {
            if (id != bodega.IdBodega)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Bodegas.FindAsync(id);

            if (existente == null)
                return NotFound("Bodega no encontrada");

            existente.Nombre = bodega.Nombre;
            existente.Ciudad = bodega.Ciudad;
            existente.Direccion = bodega.Direccion;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Bodega/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarBodega(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);

            if (bodega == null)
                return NotFound("Bodega no encontrada");

            bool tieneRelaciones =
                await _context.Existencias.AnyAsync(
                    e => e.IdBodega == id)
                || await _context.Movimientos.AnyAsync(
                    m => m.IdBodega == id)
                || await _context.Reservas.AnyAsync(
                    r => r.IdBodega == id);

            if (tieneRelaciones)
                return Conflict(
                    "No se puede eliminar una bodega con registros relacionados");

            _context.Bodegas.Remove(bodega);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
