namespace KaffaMaster.Models;

public class BasketItem
{
    public int Id { get; set; }
	public List<Product>? Products { get; set; }
    public int Count { get; set; }
    public string AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public DateTime CreatedDate { get; set; }

}
