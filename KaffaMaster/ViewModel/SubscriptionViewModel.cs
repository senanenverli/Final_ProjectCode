using System.ComponentModel.DataAnnotations;

public class SubscriptionViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

}
