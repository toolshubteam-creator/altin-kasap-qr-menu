using AltinKasap.Web.Models;

namespace AltinKasap.Web.ViewModels;

public class CategoryWithProducts
{
    public Category Category { get; set; } = null!;
    public List<Product> Products { get; set; } = new();
}
