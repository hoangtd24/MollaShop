namespace Molla.Areas.Admin.Models.Product;

public class ProductPhoto
{
    public int ProductId { get; set; }
    public required IFormFile Thumb { get; set; }
}