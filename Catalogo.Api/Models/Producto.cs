
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogo.Api.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCategoria { get; set; }

        [Range(1, int.MaxValue)]
        public int IdMarca { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999999.99")]
        public decimal Precio { get; set; }

        public bool Estado { get; set; } = true;
    }
}
