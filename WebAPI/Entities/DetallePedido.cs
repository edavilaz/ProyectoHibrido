using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ValorTotal { get; set; }

        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
