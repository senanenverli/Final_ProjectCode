namespace KaffaMaster.Models;

public class Comment
{
    public int Id { get; set; }
    public string Comments { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int? BlogId { get; set; }
    public Blog? Blog { get; set; }
}
