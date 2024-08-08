﻿using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class UserUpdateViewModel
{
//	[Required]
//	public string Fullname { get; set; }
	[Required]
	public string UserName { get; set; }
	[Required, DataType(DataType.EmailAddress)]
	public string Email { get; set; }
	[DataType(DataType.Password)]
	public string? CurrentPassword { get; set; }
	[DataType(DataType.Password), MinLength(8)]
	public string? NewPassword { get; set; }
	public string PosterImage { get; set; }
	//	[DataType(DataType.Password), MinLength(8), Compare(nameof(NewPassword))]
	//	public string? ConfirmNewPassword { get; set; }
}
