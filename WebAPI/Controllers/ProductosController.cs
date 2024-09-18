using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Entities;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;

        public ProductosController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos(string tipoProducto, int? categoriaId = null)
        {
            IEnumerable<Producto> productos;

            if (tipoProducto == "categoria" && categoriaId != null)
            {
                productos = await _productoRepository.ObtenerProductosPorCategoriaAsync(categoriaId.Value);
            }
            else if (tipoProducto == "popular")
            {
                productos = await _productoRepository.ObtenerProductosPopularesAsync();
            }
            else if (tipoProducto == "masvendido")
            {
                productos = await _productoRepository.ObtenerProductosMasVendidosAsync();
            }
            else
            {
                return BadRequest("Tipo de producto inválido");
            }

            var datosProducto = productos.Select(v => new
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Precio = v.Precio,
                UrlImagen = v.UrlImagen
            });

            return Ok(datosProducto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetalleProducto(int id)
        {
            var producto = await _productoRepository.ObtenerDetalleProductoAsync(id);

            if (producto is null)
            {
                return NotFound($"Producto con id={id} no encontrado");
            }

            var datosProducto = new
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Detalle = producto.Detalle,
                UrlImagen = producto.UrlImagen
            };

            return Ok(datosProducto);
        }
    }
}
