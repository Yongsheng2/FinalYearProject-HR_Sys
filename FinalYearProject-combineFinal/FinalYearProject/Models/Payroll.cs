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

        public float incentives_total { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public string? payback_id { get; set; }

        [ForeignKey("payback_id")]
        public virtual PayBack? PayBackDetails { get; set; }

        public string? advance_id { get; set; }

        [ForeignKey("advance_id")]
        public virtual SalaryAdvance? SalaryAdvanceDetails { get; set; }
    }
}
