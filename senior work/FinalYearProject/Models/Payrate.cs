using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Payrate
    {
        [Key]
        public string? payrate_id { get; set; }

        [Required]
        public string? payrate_name { get; set; }


        public float? payrate_ratePerHour { get; set; }
    }
}
