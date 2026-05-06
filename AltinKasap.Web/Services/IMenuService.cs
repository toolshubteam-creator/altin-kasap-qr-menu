using AltinKasap.Web.ViewModels;

namespace AltinKasap.Web.Services;

public interface IMenuService
{
    Task<MenuViewModel> GetPublicMenuAsync();
    void InvalidateCache();
}
