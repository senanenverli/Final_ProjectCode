using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class AppUserViewModel
{
    
    [Required]
    public string UserName { get; set; } = null!;
    //[Required]
    //public string FullName { get; set; } = null!;

    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
