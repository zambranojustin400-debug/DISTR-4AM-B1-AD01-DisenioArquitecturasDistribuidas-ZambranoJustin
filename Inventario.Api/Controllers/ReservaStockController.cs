
using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Inventario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaStockController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public ReservaStockController(InventarioDbContext context)
        {
            _context = context;
        }

        // GET: api/ReservaStock
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaStock>>> GetReservas()
        {
            return Ok(await _context.Reservas
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/ReservaStock/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaStock>> GetReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            return Ok(reserva);
        }

        // POST: api/ReservaStock
        [HttpPost]
        public async Task<ActionResult<ReservaStock>> CrearReserva(
            ReservaStock reserva)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existencia = await _context.Existencias
                .FirstOrDefaultAsync(e =>
                    e.IdProducto == reserva.IdProducto &&
                    e.IdBodega == reserva.IdBodega);

            if (existencia == null)
                return BadRequest(
                    "No existe stock de ese producto en la bodega");

            int reservado = await _context.Reservas
                .Where(r =>
                    r.IdProducto == reserva.IdProducto &&
                    r.IdBodega == reserva.IdBodega &&
                    r.Estado == "Activa")
                .SumAsync(r => (int?)r.Cantidad) ?? 0;

            int disponible = existencia.Stock - reservado;

            if (reserva.Cantidad > disponible)
                return BadRequest(
                    "No existe suficiente stock disponible");

            reserva.IdReserva = 0;
            reserva.Estado = "Activa";

            _context.Reservas.Add(reserva);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return CreatedAtAction(
                nameof(GetReserva),
                new { id = reserva.IdReserva },
                reserva);
        }

        // PUT: api/ReservaStock/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarReserva(
            int id, ReservaStock reserva)
        {
            if (id != reserva.IdReserva)
                return BadRequest("Los ID no coinciden");

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existente = await _context.Reservas.FindAsync(id);

            if (existente == null)
                return NotFound("Reserva no encontrada");

            if (existente.IdProducto != reserva.IdProducto ||
                existente.IdBodega != reserva.IdBodega ||
                existente.Cantidad != reserva.Cantidad)
            {
                return BadRequest(
                    "Solo se permite modificar el estado de la reserva");
            }

            if (existente.Estado != "Activa")
                return Conflict(
                    "La reserva ya fue finalizada");

            if (reserva.Estado != "Cancelada" &&
                reserva.Estado != "Confirmada")
            {
                return BadRequest(
                    "El nuevo estado debe ser Cancelada o Confirmada");
            }

            if (reserva.Estado == "Confirmada")
            {
                var existencia = await _context.Existencias
                    .FirstOrDefaultAsync(e =>
                        e.IdProducto == existente.IdProducto &&
                        e.IdBodega == existente.IdBodega);

                if (existencia == null ||
                    existencia.Stock < existente.Cantidad)
                {
                    return BadRequest("Stock insuficiente");
                }

                existencia.Stock -= existente.Cantidad;

                _context.Movimientos.Add(new MovimientoInventario
                {
                    IdProducto = existente.IdProducto,
                    IdBodega = existente.IdBodega,
                    Tipo = "Salida",
                    Cantidad = existente.Cantidad,
                    Fecha = DateTime.UtcNow
                });
            }

            existente.Estado = reserva.Estado;

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }

        // DELETE: api/ReservaStock/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva == null)
                return NotFound("Reserva no encontrada");

            if (reserva.Estado == "Confirmada")
                return Conflict(
                    "No se puede cancelar una reserva confirmada");

            reserva.Estado = "Cancelada";

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }
    }
}
