using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molla.Models;

public class Product{
    [Key]
    public int ProductId {set; get;}
    public string ProductName {set; get;} = string.Empty;
    public string Thumb {set; get;} = string.Empty;
    public string ShortDesc {set; get;} = string.Empty;
    public string Description {set; get;} = string.Empty;
    public string Alias {set; get;} = string.Empty;
    public int Price {set; get;}
    public DateTime DateCreated {set; get;}
    public DateTime DateModify {set; get;}
    public bool Active {set; get;} = true;
    public bool BestSeller {set; get;} = false;

    [Required]
    public Category Category {set; get;} = null!;

    [ForeignKey("Category")]
    public int CategoryId {set; get;}

}