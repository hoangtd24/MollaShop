using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molla.Models;

public class Photo{
    [Key]
    public int Id{get; set;}
    public string FileName {set; get;} = string.Empty;
    [ForeignKey("Product")]
    public int ProductId {set; get;}

    public Product? Product{get; set;}
    
}