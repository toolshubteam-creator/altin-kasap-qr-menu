using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, bool onlyActive = true);
    Task<Product?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<int> GetMaxSortOrderAsync(int categoryId);
}
