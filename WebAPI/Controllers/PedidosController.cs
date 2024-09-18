using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Entities;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public PedidosController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet("[action]/{pedidoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DetallesPedido(int pedidoId)
        {
            var pedidoDetalles = await (from detallePedido in dbContext.DetallesPedido
                                        join pedido in dbContext.Pedidos on detallePedido.PedidoId equals pedido.Id
                                        join producto in dbContext.Productos on detallePedido.ProductoId equals producto.Id
                                        where detallePedido.PedidoId == pedidoId
                                        select new
                                        {
                                            Id = detallePedido.Id,
                                            Cantidad = detallePedido.Cantidad,
                                            SubTotal = detallePedido.ValorTotal,
                                            ProductoNombre = producto.Nombre,
                                            ProductoImagen = producto.UrlImagen,
                                            ProductoPrecio = producto.Precio
                                        }).ToListAsync();

            if (pedidoDetalles == null || pedidoDetalles.Count == 0)
            {
                return NotFound("Detalles del pedido no encontrados.");
            }

            return Ok(pedidoDetalles);
        }



        [HttpGet("[action]/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PedidosPorUsuario(int usuarioId)
        {
            var pedidos = await (from pedido in dbContext.Pedidos
                                 where pedido.UsuarioId == usuarioId
                                 orderby pedido.FechaPedido descending
                                 select new
                                 {
                                     Id = pedido.Id,
                                     PedidoTotal = pedido.ValorTotal,
                                     FechaPedido = pedido.FechaPedido,
                                 }).ToListAsync();


            if (pedidos is null || pedidos.Count == 0)
            {
                return NotFound("No se encontraron pedidos para el usuario especificado.");
            }

            return Ok(pedidos);
        }


        //---------------------------------------------------------------------------
        // Creación del pedido y persistencioa de datos.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            pedido.FechaPedido = DateTime.Now;

            var itemsCarrito = await dbContext.ItemsCarritosCompra
                .Where(carrito => carrito.ClienteId == pedido.UsuarioId)
                .ToListAsync();

            // Verifica los items en el cartito
            if (itemsCarrito.Count == 0)
            {
                return NotFound("No hay items en el carrito.");
            }

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    dbContext.Pedidos.Add(pedido);
                    await dbContext.SaveChangesAsync();

                    foreach (var item in itemsCarrito)
                    {
                        var detallePedido = new DetallePedido()
                        {
                            Precio = item.PrecioUnitario,
                            ValorTotal = item.ValorTotal,
                            Cantidad = item.Cantidad,
                            ProductoId = item.ProductoId,
                            PedidoId = pedido.Id,
                        };
                        dbContext.DetallesPedido.Add(detallePedido);
                    }

                    await dbContext.SaveChangesAsync();
                    dbContext.ItemsCarritosCompra.RemoveRange(itemsCarrito);
                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { OrderId = pedido.Id });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Ocurrió un error al procesar el pedido.");
                }
            }
        }
    }
}
