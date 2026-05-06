using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveOrderedAsync();
    Task<Category?> GetBySlugAsync(string slug);
    Task<int> GetMaxSortOrderAsync();
}
