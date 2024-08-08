using System.ComponentModel.DataAnnotations;

namespace KaffaMaster.Areas.Admin.ViewModel
{
    public class BrandUpdateViewModel
    {
        [Required]
        public string BrandName { get; set; }
        public IFormFile? Image { get; set; }
    }
}
