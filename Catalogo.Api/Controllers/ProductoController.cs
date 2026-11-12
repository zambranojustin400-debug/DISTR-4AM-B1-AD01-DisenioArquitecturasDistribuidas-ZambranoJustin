
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly CatalogoDbContext _context;

        public ProductoController(CatalogoDbContext context)
        {
            _context = context;
        }

        // GET: api/Producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/Producto/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            return Ok(producto);
        }

        // POST: api/Producto
        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto(
            Producto producto)
        {
            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == producto.IdCategoria);

            if (!categoriaExiste)
                return BadRequest("La categoria no existe");

            bool marcaExiste = await _context.Marcas
                .AnyAsync(m => m.IdMarca == producto.IdMarca);

            if (!marcaExiste)
                return BadRequest("La marca no existe");

            bool skuExiste = await _context.Productos
                .AnyAsync(p => p.SKU == producto.SKU);

            if (skuExiste)
                return Conflict("El SKU ya esta registrado");

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.IdProducto },
                producto
            );
        }

        // PUT: api/Producto/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(
            int id, Producto producto)
        {
            if (id != producto.IdProducto)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Productos.FindAsync(id);

            if (existente == null)
                return NotFound("Producto no encontrado");

            bool categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == producto.IdCategoria);

            if (!categoriaExiste)
                return BadRequest("La categoria no existe");

            bool marcaExiste = await _context.Marcas
                .AnyAsync(m => m.IdMarca == producto.IdMarca);

            if (!marcaExiste)
                return BadRequest("La marca no existe");

            bool skuDuplicado = await _context.Productos
                .AnyAsync(p => p.SKU == producto.SKU
                    && p.IdProducto != id);

            if (skuDuplicado)
                return Conflict("El SKU ya esta registrado");

            existente.IdCategoria = producto.IdCategoria;
            existente.IdMarca = producto.IdMarca;
            existente.Nombre = producto.Nombre;
            existente.SKU = producto.SKU;
            existente.Precio = producto.Precio;
            existente.Estado = producto.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Producto/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado");

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
