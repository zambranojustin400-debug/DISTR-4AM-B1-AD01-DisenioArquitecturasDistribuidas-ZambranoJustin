
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaController : ControllerBase
    {
        private readonly CatalogoDbContext _context;

        public MarcaController(CatalogoDbContext context)
        {
            _context = context;
        }

        // GET: api/Marca
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetMarcas()
        {
            var marcas = await _context.Marcas
                .AsNoTracking()
                .ToListAsync();

            return Ok(marcas);
        }

        // GET: api/Marca/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Marca>> GetMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound("Marca no encontrada");

            return Ok(marca);
        }

        // POST: api/Marca
        [HttpPost]
        public async Task<ActionResult<Marca>> CrearMarca(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMarca),
                new { id = marca.IdMarca },
                marca
            );
        }

        // PUT: api/Marca/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarMarca(
            int id, Marca marca)
        {
            if (id != marca.IdMarca)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Marcas.FindAsync(id);

            if (existente == null)
                return NotFound("Marca no encontrada");

            existente.Nombre = marca.Nombre;
            existente.PaisOrigen = marca.PaisOrigen;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Marca/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound("Marca no encontrada");

            bool tieneProductos = await _context.Productos
                .AnyAsync(p => p.IdMarca == id);

            if (tieneProductos)
                return Conflict(
                    "No se puede eliminar una marca con productos");

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
