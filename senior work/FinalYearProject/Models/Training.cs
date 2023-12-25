using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Training
    {
        [Key]
        public string? training_id { get; set; }

        [Required]
        public string? training_name { get; set; }

        [Required]
        public DateTime? start_date { get; set; }

        [Required]
        public int? duration { get; set; }
    }
}
