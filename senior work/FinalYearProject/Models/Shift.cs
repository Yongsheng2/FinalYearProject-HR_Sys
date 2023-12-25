using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Shift
    {
        [Key]
        public string? shift_id { get; set; }

        [Required]
        public DateTime? shift_start { get; set; }

        [Required]
        public DateTime? shift_end { get; set; }

        public bool is_overtime { get; set; }

        public string? parent_shift { get; set; }

        [ForeignKey("parent_shift")]
        public virtual Shift? ParentShift { get; set; }
        
        public string? qr_code { get; set; } //tbd

        public string? payrate_id { get; set; }

        [ForeignKey("payrate_id")]
        public virtual Payrate? Payrate { get; set; }

    }
}
