using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Entities;

namespace WebAPI.Repositories
{
    public class CategoriaRepository:ICategoriaRepository
    {
        private readonly AppDbContext dbContext;

        public CategoriaRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            return await dbContext.Categorias.ToListAsync();
        }
    }
}
