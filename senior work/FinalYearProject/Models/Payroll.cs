using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Payroll
    {
        [Key]

        public string? payroll_id { get; set; }

        [Required]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Required]
        public DateTime date_created { get; set; }

        public float month_salary { get; set; }

        public float overtime_pay { get; set; }

        public float kwsp_total { get; set; }

        public float zakat_total { get; set; }
    }
}
