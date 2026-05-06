using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IAnnouncementRepository : IGenericRepository<Announcement>
{
    Task<IEnumerable<Announcement>> GetActiveAsync(DateTime now);
}
