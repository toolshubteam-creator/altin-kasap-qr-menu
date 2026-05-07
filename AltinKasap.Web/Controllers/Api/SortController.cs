using AltinKasap.Web.Data;
using AltinKasap.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Controllers.Api;

[Authorize(Roles = "Admin")]
[Route("api/sort")]
[ApiController]
public class SortController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMenuService _menuService;

    public SortController(AppDbContext db, IMenuService menuService)
    {
        _db = db;
        _menuService = menuService;
    }

    public class SortRequest
    {
        public string Type { get; set; } = "category";
        public List<int> OrderedIds { get; set; } = new();
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReorderAsync([FromBody] SortRequest req)
    {
        if (req?.OrderedIds == null || req.OrderedIds.Count == 0)
            return BadRequest(new { success = false, error = "OrderedIds gerekli." });

        if (string.Equals(req.Type, "category", StringComparison.OrdinalIgnoreCase))
        {
            var cats = await _db.Categories
                .Where(c => req.OrderedIds.Contains(c.Id))
                .ToListAsync();
            for (var i = 0; i < req.OrderedIds.Count; i++)
            {
                var cat = cats.FirstOrDefault(c => c.Id == req.OrderedIds[i]);
                if (cat != null)
                {
                    cat.SortOrder = i + 1;
                    cat.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        else if (string.Equals(req.Type, "product", StringComparison.OrdinalIgnoreCase))
        {
            var products = await _db.Products
                .Where(p => req.OrderedIds.Contains(p.Id))
                .ToListAsync();
            for (var i = 0; i < req.OrderedIds.Count; i++)
            {
                var p = products.FirstOrDefault(x => x.Id == req.OrderedIds[i]);
                if (p != null)
                {
                    p.SortOrder = i + 1;
                    p.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        else
        {
            return BadRequest(new { success = false, error = "Type 'category' veya 'product' olmalı." });
        }

        await _db.SaveChangesAsync();
        _menuService.InvalidateCache();
        return Ok(new { success = true });
    }
}
