
using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public MovimientoInventarioController(
            InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/MovimientoInventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventario>>> GetMovimientos()
        {
            return Ok(await _context.Movimientos
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/MovimientoInventario/1
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventario>> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
                return NotFound("Movimiento no encontrado");

            return Ok(movimiento);
        }

        // POST: api/MovimientoInventario
        [HttpPost]
        public async Task<ActionResult<MovimientoInventario>> CrearMovimiento(
            MovimientoInventario movimiento)
        {
            string tipo = movimiento.Tipo.Trim();

            if (tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                tipo = "Entrada";
            else if (tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                tipo = "Salida";
            else
                return BadRequest("El tipo debe ser Entrada o Salida");

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existencia = await _context.Existencias
                .FirstOrDefaultAsync(e =>
                    e.IdProducto == movimiento.IdProducto &&
                    e.IdBodega == movimiento.IdBodega);

            if (existencia == null)
                return BadRequest(
                    "Primero debe existir el producto en esa bodega");

            int reservado = await _context.Reservas
                .Where(r =>
                    r.IdProducto == movimiento.IdProducto &&
                    r.IdBodega == movimiento.IdBodega &&
                    r.Estado == "Activa")
                .SumAsync(r => (int?)r.Cantidad) ?? 0;

            if (tipo == "Salida")
            {
                int disponible = existencia.Stock - reservado;

                if (movimiento.Cantidad > disponible)
                    return BadRequest(
                        "Stock disponible insuficiente para la salida");

                existencia.Stock -= movimiento.Cantidad;
            }
            else
            {
                if (existencia.Stock >
                    int.MaxValue - movimiento.Cantidad)
                    return BadRequest("La cantidad supera el limite permitido");

                existencia.Stock += movimiento.Cantidad;
            }

            movimiento.IdMovimiento = 0;
            movimiento.Tipo = tipo;
            movimiento.Fecha = DateTime.UtcNow;

            _context.Movimientos.Add(movimiento);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return CreatedAtAction(
                nameof(GetMovimiento),
                new { id = movimiento.IdMovimiento },
                movimiento);
        }
    }
}
