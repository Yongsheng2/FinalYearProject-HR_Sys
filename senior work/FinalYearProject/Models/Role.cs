using FinalYearProject.Utility;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    [Authorize(Roles = SD.RoleManage)]
    public class Role
    {
        [Key]
        [Display(Name = "Role ID")]
        public string? role_id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string? role_name { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTime? date_created { get; set; }

        [Display(Name = "Company")]
        public string? company_id { get; set; }

        [ForeignKey("company_id")]
        public virtual Company? Company { get; set; }
    }
}
