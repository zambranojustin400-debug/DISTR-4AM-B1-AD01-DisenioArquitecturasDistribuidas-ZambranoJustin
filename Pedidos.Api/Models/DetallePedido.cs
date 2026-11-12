
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Api.Models
{
    public class DetallePedido
    {
        [Key]
        public int IdDetalle { get; set; }

        [Range(1, int.MaxValue)]
        public int IdPedido { get; set; }

        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999999.99",
           ParseLimitsInInvariantCulture = true)]
        public decimal PrecioUnitario { get; set; }
    }
}
