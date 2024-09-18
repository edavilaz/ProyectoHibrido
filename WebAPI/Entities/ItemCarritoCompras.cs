using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entities
{
    public class ItemCarritoCompras
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal ValorTotal { get; set; }

        public int ProductoId { get; set; }
        public int ClienteId { get; set; }
    }
}
