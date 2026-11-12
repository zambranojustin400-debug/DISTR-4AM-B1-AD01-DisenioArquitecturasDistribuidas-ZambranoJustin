
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;
using System.Data;

namespace Pedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallePedidoController : ControllerBase
    {
        private readonly PedidosDbContext _context;

        private const decimal MaximoTotal =
            9999999999999999.99m;

        public DetallePedidoController(PedidosDbContext context)
        {
            _context = context;
        }

        // GET: api/DetallePedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
        {
            return Ok(await _context.DetallesPedido
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/DetallePedido/1
        [HttpGet("{id}")]
        public async Task<ActionResult<DetallePedido>> GetDetalle(int id)
        {
            var detalle = await _context.DetallesPedido.FindAsync(id);

            if (detalle == null)
                return NotFound("Detalle no encontrado");

            return Ok(detalle);
        }

        // POST: api/DetallePedido
        [HttpPost]
        public async Task<ActionResult<DetallePedido>> CrearDetalle(
            DetallePedido detalle)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var pedido = await _context.Pedidos
                .FindAsync(detalle.IdPedido);

            if (pedido == null)
                return BadRequest("El pedido no existe");

            if (pedido.Estado != "Pendiente")
                return Conflict(
                    "Solo se agregan detalles a pedidos pendientes");

            decimal subtotal =
                detalle.Cantidad * detalle.PrecioUnitario;

            if (pedido.Total + subtotal > MaximoTotal)
                return BadRequest(
                    "El total supera el limite permitido");

            detalle.IdDetalle = 0;

            _context.DetallesPedido.Add(detalle);

            pedido.Total += subtotal;

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return CreatedAtAction(
                nameof(GetDetalle),
                new { id = detalle.IdDetalle },
                detalle);
        }

        // PUT: api/DetallePedido/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarDetalle(
            int id, DetallePedido detalle)
        {
            if (id != detalle.IdDetalle)
                return BadRequest("Los ID no coinciden");

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var existente = await _context.DetallesPedido
                .FindAsync(id);

            if (existente == null)
                return NotFound("Detalle no encontrado");

            if (existente.IdPedido != detalle.IdPedido)
                return BadRequest(
                    "No se puede cambiar el pedido del detalle");

            var pedido = await _context.Pedidos
                .FindAsync(existente.IdPedido);

            if (pedido == null)
                return NotFound("Pedido no encontrado");

            if (pedido.Estado != "Pendiente")
                return Conflict(
                    "No se puede modificar un pedido finalizado");

            decimal subtotalAnterior =
                existente.Cantidad * existente.PrecioUnitario;

            decimal subtotalNuevo =
                detalle.Cantidad * detalle.PrecioUnitario;

            decimal nuevoTotal =
                pedido.Total - subtotalAnterior + subtotalNuevo;

            if (nuevoTotal < 0 || nuevoTotal > MaximoTotal)
                return BadRequest("Total fuera del rango permitido");

            existente.IdProducto = detalle.IdProducto;
            existente.Cantidad = detalle.Cantidad;
            existente.PrecioUnitario = detalle.PrecioUnitario;

            pedido.Total = nuevoTotal;

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }

        // DELETE: api/DetallePedido/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarDetalle(int id)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var detalle = await _context.DetallesPedido
                .FindAsync(id);

            if (detalle == null)
                return NotFound("Detalle no encontrado");

            var pedido = await _context.Pedidos
                .FindAsync(detalle.IdPedido);

            if (pedido == null)
                return NotFound("Pedido no encontrado");

            if (pedido.Estado != "Pendiente")
                return Conflict(
                    "No se puede modificar un pedido finalizado");

            decimal subtotal =
                detalle.Cantidad * detalle.PrecioUnitario;

            pedido.Total -= subtotal;

            _context.DetallesPedido.Remove(detalle);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return NoContent();
        }
    }
}
