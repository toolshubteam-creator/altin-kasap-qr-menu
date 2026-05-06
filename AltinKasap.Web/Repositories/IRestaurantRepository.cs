using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IRestaurantRepository : IGenericRepository<Restaurant>
{
    Task<Restaurant?> GetSingleAsync();
}
