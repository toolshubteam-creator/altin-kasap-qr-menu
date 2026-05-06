using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IDailySpecialRepository : IGenericRepository<DailySpecial>
{
    Task<DailySpecial?> GetForDateAsync(DateTime date);
}
