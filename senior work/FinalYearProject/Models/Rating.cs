using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Rating
    {
        [Key]
        public string? rating_id { get; set; }

        [Required]
        public string? staff_rated { get; set; }

        [ForeignKey("staff_rated")]
        public virtual EmployeeDetails? EmployeeRated { get; set; }

        public string? rated_by { get; set; }

        [ForeignKey("rated_by")]
        public virtual EmployeeDetails? RateByEmployee { get; set; }

        [Required]
        [Range(1, 5)]
        public int rating { get; set; }
    }
}
