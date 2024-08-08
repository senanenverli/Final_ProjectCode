﻿using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class ForgotPasswordViewModel
{
    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
