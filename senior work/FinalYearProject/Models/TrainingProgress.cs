using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class TrainingProgress
    {
        [Key]
        public string? staff_id { get; set; }

        [ForeignKey("staff_id")]
        public virtual EmployeeDetails? EmployeeDetails { get; set; }

        [Key]
        public string? training_id { get; set; }

        [ForeignKey("training_id")]
        public virtual Training? Training { get; set; }

        public bool completion { get; set; }

        public int duration_left { get; set; }

        public string cert_id { get; set; }

        [ForeignKey("cert_id")]
        public virtual Document certification { get; set; }

    }
}
