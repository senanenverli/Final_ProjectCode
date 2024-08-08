using KaffaMaster.Models;

namespace WebApplication6.Areas.Admin.ViewModel
{
    public class UserViewModel
    {
        public AppUser User { get; set; }
        public IList<string> Roles { get; set; } = null!;
    }
}
