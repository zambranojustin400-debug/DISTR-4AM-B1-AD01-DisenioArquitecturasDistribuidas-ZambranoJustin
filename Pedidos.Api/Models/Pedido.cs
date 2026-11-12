
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Api.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCliente { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "999999999.99",
          ParseLimitsInInvariantCulture = true)]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";
    }
}
