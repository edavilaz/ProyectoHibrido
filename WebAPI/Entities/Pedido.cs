using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string? Direccion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ValorTotal { get; set; }
        public DateTime FechaPedido { get; set; }
        public int UsuarioId { get; set; }
        public ICollection<DetallePedido>? DetallesPedido { get; set; }
    }
}
