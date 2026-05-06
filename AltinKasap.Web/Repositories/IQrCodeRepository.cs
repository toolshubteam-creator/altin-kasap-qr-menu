using AltinKasap.Web.Models;

namespace AltinKasap.Web.Repositories;

public interface IQrCodeRepository : IGenericRepository<QrCode>
{
    Task<IEnumerable<QrCode>> GetAllWithScanCountAsync();
}
