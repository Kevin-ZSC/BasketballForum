using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BasketballForum.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Required]
        public string Name { get; set; } = string.Empty;

        [PersonalData]
        public string Location { get; set; } = string.Empty;

        [PersonalData]
        public string ImageFilename { get; set; } = string.Empty;

        [PersonalData]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
