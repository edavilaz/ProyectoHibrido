using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Context;
using WebAPI.Entities;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsCarritoCompraController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ItemsCarritoCompraController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Get(int usuarioId)
        {
            var user = await dbContext.Usuarios.FindAsync(usuarioId);

            if (user is null)
            {
                return NotFound($"Usuario con id = {usuarioId} no encontrado");
            }

            var itemsCarrito = await (from s in dbContext.ItemsCarritosCompra.Where(s => s.ClienteId == usuarioId)
                                      join p in dbContext.Productos on s.ProductoId equals p.Id
                                      select new
                                      {
                                          Id = s.Id,
                                          Precio = s.PrecioUnitario,
                                          ValorTotal = s.ValorTotal,
                                          Cantidad = s.Cantidad,
                                          ProductoId = p.Id,
                                          ProductoNombre = p.Nombre,
                                          UrlImagen = p.UrlImagen
                                      }).ToListAsync();

            return Ok(itemsCarrito);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ItemCarritoCompras itemCarritoCompra)
        {
            try
            {
                var carritoCompra = await dbContext.ItemsCarritosCompra.FirstOrDefaultAsync(s =>
                                        s.ProductoId == itemCarritoCompra.ProductoId &&
                                        s.ClienteId == itemCarritoCompra.ClienteId);

                if (carritoCompra is not null)
                {
                    carritoCompra.Cantidad += itemCarritoCompra.Cantidad;
                    carritoCompra.ValorTotal = carritoCompra.PrecioUnitario * carritoCompra.Cantidad;
                }
                else
                {
                    var producto = await dbContext.Productos.FindAsync(itemCarritoCompra.ProductoId);

                    var carrito = new ItemCarritoCompras()
                    {
                        ClienteId = itemCarritoCompra.ClienteId,
                        ProductoId = itemCarritoCompra.ProductoId,
                        PrecioUnitario = itemCarritoCompra.PrecioUnitario,
                        Cantidad = itemCarritoCompra.Cantidad,
                        ValorTotal = (producto!.Precio) * (itemCarritoCompra.Cantidad)
                    };
                    dbContext.ItemsCarritosCompra.Add(carrito);
                }
                await dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocurrió un error al procesar la solicitud.");
            }
        }

        /// <summary>
        /// Atualizar la cantidad de um item.
        /// </summary>
        /// <param name="productoId"> ID del producto.</param>

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<IActionResult> Put(int productoId, string accion)
        {
            // Este codigo recupera email del usuario autenticado con token JWT decodificado,
            // Claims representa las declaraciones associadas del usuario autenticado

            var usuarioEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioEmail);

            if (usuario is null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var itemCarritoCompras = await dbContext.ItemsCarritosCompra.FirstOrDefaultAsync(s =>
                                                   s.ClienteId == usuario!.Id && s.ProductoId == productoId);

            if (itemCarritoCompras != null)
            {
                if (accion.ToLower() == "aumentar")
                {
                    itemCarritoCompras.Cantidad += 1;
                }
                else if (accion.ToLower() == "disminuir")
                {
                    if (itemCarritoCompras.Cantidad > 1)
                    {
                        itemCarritoCompras.Cantidad -= 1;
                    }
                    else
                    {
                        dbContext.ItemsCarritosCompra.Remove(itemCarritoCompras);
                        await dbContext.SaveChangesAsync();
                        return Ok();
                    }
                }
                else if (accion.ToLower() == "borrar")
                {
                    // Remove item 
                    dbContext.ItemsCarritosCompra.Remove(itemCarritoCompras);
                    await dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Acción Inválida. Use : 'aumentar', 'disminuir' o 'borrar' para realizar una acción");
                }

                itemCarritoCompras.ValorTotal = itemCarritoCompras.PrecioUnitario * itemCarritoCompras.Cantidad;
                await dbContext.SaveChangesAsync();
                return Ok($"Operación : {accion} realizada,");
            }
            else
            {
                return NotFound("No hay items en el carrito");
            }
        }
    }
}
