using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IPriceHistoryRepository : IGenericRepository<PriceHistory>
{
    Task<IEnumerable<PriceHistory>> GetForProductAsync(int productId);
}
