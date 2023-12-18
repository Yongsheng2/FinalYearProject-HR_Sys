using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class SalaryAdvance
    {
        [Key]
        public string? advance_id { get; set; }
        
        public string? employee_id { get; set; }

        [ForeignKey("employee_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        public string? reason { get; set; }

        public float? amount { get; set; }

        public int? time_to_payback { get; set; }

        public string? status { get; set; }

        public DateTime? request_date { get; set; }

        public string? approved_by { get; set; }

        [ForeignKey("approved_by")]
        public virtual EmployeeDetails? ApprovedEmployeeDetails { get; set; }
    }
}
