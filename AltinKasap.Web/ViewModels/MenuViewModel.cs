using AltinKasap.Web.Models;

namespace AltinKasap.Web.ViewModels;

public class MenuViewModel
{
    public Restaurant Restaurant { get; set; } = null!;
    public List<CategoryWithProducts> Categories { get; set; } = new();
    public List<Announcement> ActiveAnnouncements { get; set; } = new();
    public DailySpecial? TodaysSpecial { get; set; }
}
