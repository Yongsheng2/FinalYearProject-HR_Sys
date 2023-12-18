using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Survey
    {
        [Display(Name = "survey_id")]
        [Key]
        public string? survey_id { get; set; }


        [Required]
        public string? company_id { get; set; }
        [ForeignKey("company_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Required]
        public string? survey_name { get; set; }
    }
}
