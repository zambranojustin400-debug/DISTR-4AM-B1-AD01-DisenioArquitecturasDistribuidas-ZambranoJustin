
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly PedidosDbContext _context;

        public PedidoController(PedidosDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return Ok(await _context.Pedidos
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/Pedido/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
                return NotFound("Pedido no encontrado");

            return Ok(pedido);
        }

        // POST: api/Pedido
        [HttpPost]
        public async Task<ActionResult<Pedido>> CrearPedido(Pedido pedido)
        {
            bool clienteExiste = await _context.Clientes
                .AnyAsync(c => c.IdCliente == pedido.IdCliente);

            if (!clienteExiste)
                return BadRequest("El cliente no existe");

            pedido.IdPedido = 0;
            pedido.Fecha = DateTime.UtcNow;
            pedido.Total = 0;
            pedido.Estado = "Pendiente";

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPedido),
                new { id = pedido.IdPedido },
                pedido);
        }

        // PUT: api/Pedido/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPedido(
            int id, Pedido pedido)
        {
            if (id != pedido.IdPedido)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Pedidos.FindAsync(id);

            if (existente == null)
                return NotFound("Pedido no encontrado");

            if (existente.Estado != "Pendiente")
                return Conflict(
                    "Solo se pueden modificar pedidos pendientes");

            string estado = pedido.Estado.Trim();

            if (estado != "Pendiente" &&
                estado != "Confirmado" &&
                estado != "Cancelado")
            {
                return BadRequest("Estado no permitido");
            }

            bool clienteExiste = await _context.Clientes
                .AnyAsync(c => c.IdCliente == pedido.IdCliente);

            if (!clienteExiste)
                return BadRequest("El cliente no existe");

            bool tieneDetalles = await _context.DetallesPedido
                .AnyAsync(d => d.IdPedido == id);

            if (tieneDetalles &&
                pedido.IdCliente != existente.IdCliente)
            {
                return BadRequest(
                    "No se puede cambiar el cliente de un pedido con detalles");
            }

            if (estado == "Confirmado" && !tieneDetalles)
                return BadRequest(
                    "No se puede confirmar un pedido sin detalles");

            existente.IdCliente = pedido.IdCliente;
            existente.Estado = estado;

            // Fecha y Total no se modifican directamente
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Pedido/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
                return NotFound("Pedido no encontrado");

            if (pedido.Estado != "Pendiente")
                return Conflict(
                    "Solo se pueden eliminar pedidos pendientes");

            bool tieneDetalles = await _context.DetallesPedido
                .AnyAsync(d => d.IdPedido == id);

            if (tieneDetalles)
                return Conflict(
                    "Primero debe eliminar los detalles del pedido");

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
