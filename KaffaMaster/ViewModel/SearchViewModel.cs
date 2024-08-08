using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.ViewModel;

public class SearchViewModel
{
    [Required]
    public string SearchTerm { get; set; }
}