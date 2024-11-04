namespace Molla.Areas.Admin.Models.Product;

public class ProductViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ShortDesc { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public int Price { get; set; }
    public bool Active { get; set; } = true;
    public bool BestSeller { get; set; } = false;
    public int CategoryId { get; set; }

    public IFormFile? ThumbFile { get; set; }  // Sử dụng để upload ảnh
}