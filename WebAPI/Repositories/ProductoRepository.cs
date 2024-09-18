using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Entities;

namespace WebAPI.Repositories
{
    public class ProductoRepository:IProductoRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosPorCategoriaAsync(int categoriaId)
        {
            return await _dbContext.Productos
                .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosPopularesAsync()
        {
            return await _dbContext.Productos
                .Where(p => p.Popular)
            .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosMasVendidosAsync()
        {
            return await _dbContext.Productos
                .Where(p => p.MasVendido)
            .ToListAsync();
        }

        public async Task<Producto> ObtenerDetalleProductoAsync(int id)
        {
            var detalleProducto = await _dbContext.Productos
                                                  .FirstOrDefaultAsync(p => p.Id == id);

            if (detalleProducto is null)
                throw new InvalidOperationException();

            return detalleProducto;
        }
    }
}
