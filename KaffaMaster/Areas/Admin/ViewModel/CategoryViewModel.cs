using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.Areas.Admin.ViewModel
{
    public class CategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
