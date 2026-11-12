
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CatalogoDbContext _context;

        public CategoriaController(CatalogoDbContext context)
        {
            _context = context;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/Categoria/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoria no encontrada");

            return Ok(categoria);
        }

        // POST: api/Categoria
        [HttpPost]
        public async Task<ActionResult<Categoria>> CrearCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                categoria
            );
        }

        // PUT: api/Categoria/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCategoria(
            int id, Categoria categoria)
        {
            if (id != categoria.IdCategoria)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Categorias.FindAsync(id);

            if (existente == null)
                return NotFound("Categoria no encontrada");

            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categoria/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoria no encontrada");

            bool tieneProductos = await _context.Productos
                .AnyAsync(p => p.IdCategoria == id);

            if (tieneProductos)
                return Conflict(
                    "No se puede eliminar una categoria con productos");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
