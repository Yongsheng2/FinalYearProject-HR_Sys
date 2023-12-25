using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Benefit
    {
        [Key]
        public string? benefit_id { get; set; }

        [Required]
        public string? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        public string? benefit_desc { get; set; }

        public string? benefit_type { get; set; }

        [Required]
        public DateTime? start_date { get; set; }

        [Required]
        public DateTime? end_date { get; set; }

        [Required]
        public int days { get; set; }
    }
}
