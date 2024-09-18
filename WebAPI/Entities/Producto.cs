using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Required]
        public string? Nombre { get; set; }
        [StringLength(200)]
        [Required]
        public string? Detalle { get; set; }
        [StringLength(200)]
        [Required]
        public string? UrlImagen { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }
        public bool Popular { get; set; }
        public bool MasVendido { get; set; }
        public bool Disponible { get; set; }
        public int CategoriaId { get; set; }

        [JsonIgnore]
        public ICollection<DetallePedido>? DetallesPedido { get; set; }
        [JsonIgnore]
        public ICollection<ItemCarritoCompras>? ItemsCarritoCompras { get; set; }
    }
}
