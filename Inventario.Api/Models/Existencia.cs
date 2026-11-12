
using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models
{
    public class Existencia
    {
        [Key]
        public int IdExistencia { get; set; }

        [Range(1, int.MaxValue)]
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue)]
        public int IdBodega { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
