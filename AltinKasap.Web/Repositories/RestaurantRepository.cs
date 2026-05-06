using AltinKasap.Web.Data;
using AltinKasap.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext db) : base(db) { }

    public Task<Restaurant?> GetSingleAsync() =>
        Set.OrderBy(r => r.Id).FirstOrDefaultAsync();
}
