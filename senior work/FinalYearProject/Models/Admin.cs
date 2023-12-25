using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Admin
    {
        [Display(Name = "Admin ID")]
        [Key]
        public string? admin_id { get; set; }

        
        [Required]
        public string? admin_pass { get; set; }

        [Required]
        public bool is_superadmin { get; set; }

    }
}
