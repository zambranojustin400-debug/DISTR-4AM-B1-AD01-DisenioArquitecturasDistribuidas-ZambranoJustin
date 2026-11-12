
using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models
{
    public class Bodega
    {
        [Key]
        public int IdBodega { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;
    }
}
