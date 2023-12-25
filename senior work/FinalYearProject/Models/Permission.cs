using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Permission
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string baseRole { get; set; }

        [ForeignKey("baseRole")]
        public virtual Role Role { get; set; }

        [Required]
        public string identityRole { get; set; }

    }
}
