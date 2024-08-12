using Microsoft.AspNetCore.Identity;

namespace KaffaMaster.Models;

public class AppUser : IdentityUser
{

    //public string Fullname { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsSubscribed { get; set; }
}
