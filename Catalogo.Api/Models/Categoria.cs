
using System.ComponentModel.DataAnnotations;

namespace Catalogo.Api.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Descripcion { get; set; }
    }
}
