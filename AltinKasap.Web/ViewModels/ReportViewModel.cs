namespace AltinKasap.Web.ViewModels;

public class ReportViewModel
{
    public string[] Labels { get; set; } = Array.Empty<string>();
    public long[] Data { get; set; } = Array.Empty<long>();
    public long Total { get; set; }
    public string Title { get; set; } = string.Empty;
}
