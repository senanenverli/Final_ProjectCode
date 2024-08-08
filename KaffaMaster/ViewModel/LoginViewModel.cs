﻿using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class LoginViewModel
{
    [Required]
    public string UsernameOrEmail { get; set; } = null!;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
