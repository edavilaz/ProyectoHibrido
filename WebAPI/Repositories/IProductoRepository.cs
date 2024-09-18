using WebAPI.Entities;

namespace WebAPI.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerProductosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Producto>> ObtenerProductosPopularesAsync();
        Task<IEnumerable<Producto>> ObtenerProductosMasVendidosAsync();
        Task<Producto> ObtenerDetalleProductoAsync(int id);
    }
}
