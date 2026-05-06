using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IProductTagRepository : IGenericRepository<ProductTag>
{
    Task<IEnumerable<ProductTag>> GetAllOrderedAsync();
}
