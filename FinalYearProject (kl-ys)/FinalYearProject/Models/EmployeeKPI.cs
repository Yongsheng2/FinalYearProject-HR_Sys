using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class EmployeeKPI
    {
        [Key, Column(Order = 0)] // Specify the order for the composite key
        public string? employee_id { get; set; }

        [Key, Column(Order = 1)] // Specify the order for the composite key
        public string? KPI_id { get; set; }



    }
}
