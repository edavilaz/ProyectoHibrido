using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        //[Required]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(150)]
        [Required]
        public string? Email { get; set; }

        [StringLength(100)]
        [Required]
        public string? Password { get; set; }

        [StringLength(100)]
        public string? UrlImagen { get; set; }

        [NotMapped]
        public IFormFile? Imagen { get; set; }


        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
