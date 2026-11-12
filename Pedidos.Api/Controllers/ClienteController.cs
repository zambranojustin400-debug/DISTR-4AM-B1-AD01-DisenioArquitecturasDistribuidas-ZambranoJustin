
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly PedidosDbContext _context;

        public ClienteController(PedidosDbContext context)
        {
            _context = context;
        }

        // GET: api/Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return Ok(await _context.Clientes
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: api/Cliente/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            return Ok(cliente);
        }

        // POST: api/Cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente(Cliente cliente)
        {
            bool existe = await _context.Clientes
                .AnyAsync(c => c.Cedula == cliente.Cedula);

            if (existe)
                return Conflict("La cedula ya esta registrada");

            cliente.IdCliente = 0;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.IdCliente },
                cliente);
        }

        // PUT: api/Cliente/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCliente(
            int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return BadRequest("Los ID no coinciden");

            var existente = await _context.Clientes.FindAsync(id);

            if (existente == null)
                return NotFound("Cliente no encontrado");

            bool duplicado = await _context.Clientes
                .AnyAsync(c =>
                    c.Cedula == cliente.Cedula &&
                    c.IdCliente != id);

            if (duplicado)
                return Conflict("La cedula ya esta registrada");

            existente.Nombres = cliente.Nombres;
            existente.Cedula = cliente.Cedula;
            existente.Correo = cliente.Correo;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cliente/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            bool tienePedidos = await _context.Pedidos
                .AnyAsync(p => p.IdCliente == id);

            if (tienePedidos)
                return Conflict(
                    "No se puede eliminar un cliente con pedidos");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
