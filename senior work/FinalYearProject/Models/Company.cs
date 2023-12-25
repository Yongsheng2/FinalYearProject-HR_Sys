using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Company
    {
        [Key]
        public string? company_id { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string? company_name { get; set; }

        [Required]
        [Display(Name = "Creation Date")]
        public DateTime date_created { get; set; }

        [Required]
        [Display(Name = "Admin")]
        public string? current_admin { get; set; }

        [ForeignKey("current_admin")]
        public virtual Admin? Admin { get; set; }

        [Display(Name = "Address")]
        public string? address { get; set; }

        [Display(Name = "Longitude")]
        public string? longitude { get; set; }

        [Display(Name = "Latitude")]
        public string? latitude { get; set; }

        [Display(Name = "Company Radius")]
        public string? radius { get; set; }

        [Display(Name = "Late Grace Period (Hours)")]
        public int late_gracePeriod { get; set; }

        [Required]
        public int paidLeaveYearly { get; set; }

        [Required]
        public int sickLeaveYearly { get; set; }

        [Required]
        public int paidMaxCarryover { get; set; }

        [Required]
        public int sickMaxCarryover { get; set; }

        [Required]
        public int leaveHoursPerDay { get; set; }
    }
}
