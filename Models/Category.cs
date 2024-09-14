using System.ComponentModel.DataAnnotations;

namespace Molla.Models;

public class Category{
    [Key]
    public int CategoryId {get; set;}
    public string CatName {set; get;} = string.Empty;

    public string Description {set; get;} = string.Empty;

    public bool Published {set; get;}

    public string Alias{set; get;} = string.Empty;
}