
using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExistenciaController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public ExistenciaController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/Existencia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Existencia>>> GetExistencias()
        {
            return Ok(await _context.Existencias
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/Existencia/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Existencia>> GetExistencia(int id)
        {
            var existencia = await _context.Existencias.FindAsync(id);

            if (existencia == null)
                return NotFound("Existencia no encontrada");

            return Ok(existencia);
        }

        // POST: api/Existencia
        [HttpPost]
        public async Task<ActionResult<Existencia>> CrearExistencia(
            Existencia existencia)
        {
            bool bodegaExiste = await _context.Bodegas
                .AnyAsync(b => b.IdBodega == existencia.IdBodega);

            if (!bodegaExiste)
                return BadRequest("La bodega no existe");

            bool duplicado = await _context.Existencias.AnyAsync(
                e => e.IdProducto == existencia.IdProducto
                  && e.IdBodega == existencia.IdBodega);

            if (duplicado)
                return Conflict(
                    "El producto ya tiene una existencia en esa bodega");

            existencia.IdExistencia = 0;

            _context.Existencias.Add(existencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetExistencia),
                new { id = existencia.IdExistencia },
                existencia);
        }

        // PUT: api/Existencia/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarExistencia(
            int id, Existencia existencia)
        {
            if (id != existencia.IdExistencia)
                return BadRequest("Los ID no coinciden");

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existente = await _context.Existencias.FindAsync(id);

            if (existente == null)
                return NotFound("Existencia no encontrada");

            if (existente.IdProducto != existencia.IdProducto ||
                existente.IdBodega != existencia.IdBodega)
            {
                return BadRequest(
                    "No se puede cambiar el producto o la bodega de una existencia");
            }

            int reservado = await _context.Reservas
                .Where(r =>
                    r.IdProducto == existente.IdProducto &&
                    r.IdBodega == existente.IdBodega &&
                    r.Estado == "Activa")
                .SumAsync(r => (int?)r.Cantidad) ?? 0;

            if (existencia.Stock < reservado)
                return BadRequest(
                    "El stock no puede ser menor que la cantidad reservada");

            existente.Stock = existencia.Stock;

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }

        // DELETE: api/Existencia/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarExistencia(int id)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existencia = await _context.Existencias.FindAsync(id);

            if (existencia == null)
                return NotFound("Existencia no encontrada");

            bool tieneMovimientos = await _context.Movimientos
                .AnyAsync(m =>
                    m.IdProducto == existencia.IdProducto &&
                    m.IdBodega == existencia.IdBodega);

            bool tieneReservas = await _context.Reservas
                .AnyAsync(r =>
                    r.IdProducto == existencia.IdProducto &&
                    r.IdBodega == existencia.IdBodega);

            if (tieneMovimientos || tieneReservas)
                return Conflict(
                    "No se puede eliminar una existencia con movimientos o reservas");

            _context.Existencias.Remove(existencia);
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }
    }
}
