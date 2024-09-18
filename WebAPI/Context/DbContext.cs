using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;

namespace WebAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ItemCarritoCompras> ItemsCarritosCompra { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Servicios", UrlImagen = "servicios.png" },
                new Categoria { Id = 2, Nombre = "Talleres", UrlImagen = "talleres.png" }
                );

            modelBuilder.Entity<Producto>().HasData(
               new Producto { Id = 1, Nombre = "Desayunos", UrlImagen = "desayuno.png", CategoriaId = 1, Precio = 150000, Disponible = true, MasVendido = true, Popular = true, Detalle = "Desayunos sanos y deliciosos. " },
               new Producto { Id = 2, Nombre = "Chef a Domicilio", UrlImagen = "chef.png", CategoriaId = 1, Precio = 450000, Disponible = true, MasVendido = true, Popular = true, Detalle = "¿Quieres que preparemos las delicias en tu hogar?. " },
               new Producto { Id = 3, Nombre = "Catering", UrlImagen = "catering.png", CategoriaId = 1, Precio = 750000, Disponible = true, MasVendido = true, Popular = true, Detalle = "Atendemos tus eventos sociales o de trabajo. " },
               new Producto { Id = 4, Nombre = "Cocina Mexicana", UrlImagen = "mexicana.png", CategoriaId = 2, Precio = 250000, Disponible = true, MasVendido = true, Popular = true, Detalle = "Exquisitos Tacos, Tortillas y más delicias. " },
               new Producto { Id = 5, Nombre = "Cocina Colombiana", UrlImagen = "colombia.png", CategoriaId = 2, Precio = 250000, Disponible = true, MasVendido = true, Popular = true, Detalle = "Delicias de nuestra tierra. " }
                );

        }
    }
}
