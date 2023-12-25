using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Compensation
    {
        [Key]
        public int comp_id { get; set; }

        [Required]
        public string? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        public string? comp_type { get; set; }

        public string? comp_desc { get; set; }

        [Required]
        public DateTime date_applied { get; set; }

        public string? approved_by { get; set; }

        public string? status { get; set; }

        public string? reject_reason { get; set; }

        public DateTime date_completed { get; set; }

    }
}
