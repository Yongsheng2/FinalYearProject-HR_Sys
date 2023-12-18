using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class CompanyKPI
    {
        [Key]
        public string? KPI_id { get; set; }


        public string? target_name { get; set; }

        public string? company_id { get; set; }
        [ForeignKey("company_id")]
        public virtual Company? Company { get; set; }

        [DisplayName("Target_KPI(RM)")]
        public decimal? target_KPI { get; set; }


        public string? status { get; set;}

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        public decimal? hit_KPI_allowance { get; set; }
    }
}
