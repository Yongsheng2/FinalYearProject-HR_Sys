using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class EmployeeIncentives
    {
        [Key, Column(Order = 0)] // Specify the order for the composite key
        public string? employee_id { get; set; }

        [Key, Column(Order = 1)] // Specify the order for the composite key
        public string? incentives_id { get; set; }

        [Key, Column(Order = 2)] // Specify the order for the composite key
        public DateTime? start_Claimed { get; set; }

        [Key, Column(Order = 3)] // Specify the order for the composite key
        public DateTime? end_Claimed { get; set; }

    }
}
