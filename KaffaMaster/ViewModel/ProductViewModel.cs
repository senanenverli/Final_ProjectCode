using KaffaMaster.Models;
using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class ProductViewModel
{
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public double Price { get; set; }
    [Required]
    public double Rating { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    [Required]
    public IFormFile PosterImage { get; set; }
    //public List<IFormFile> Images { get; set; }
}
