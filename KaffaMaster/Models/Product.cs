namespace KaffaMaster.Models;

public class Product
{

    public int Id { get; set; }
    
    public string Title { get; set; }

    public double Price { get; set; }

    public double Rating { get; set; }
    public string PosterImage { get; set; }

    public bool isDeleted { get; set; }

    public bool isStocked { get; set; }
    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    public List<BasketItem>? BasketItems { get; set; }
}
