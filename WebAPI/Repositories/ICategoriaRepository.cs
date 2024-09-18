using WebAPI.Entities;

namespace WebAPI.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetCategorias();
    }
}
