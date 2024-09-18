using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? Nombre { get; set; }
        [StringLength(200)]
        [Required]
        public string? UrlImagen { get; set; }

        [JsonIgnore]
        public ICollection<Producto>? Productos { get; set; }
    }
}
