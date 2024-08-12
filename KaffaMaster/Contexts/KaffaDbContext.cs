using KaffaMaster.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KaffaMaster.Contexts;

public class KaffaDbContext : IdentityDbContext<AppUser>
{
    public KaffaDbContext(DbContextOptions<KaffaDbContext> options) : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> productimages { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Order> Orders { get; set; }

    //public DbSet<Setting> Settings { get; set; } = null!;
}
