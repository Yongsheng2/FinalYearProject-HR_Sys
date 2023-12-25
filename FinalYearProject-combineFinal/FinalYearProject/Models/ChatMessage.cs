using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class ChatMessage
    {
        [Key]
        public string? chatmsg_id { get; set; }

        [Display(Name = "Chat ID")]
        public string chat_id { get; set; }

        [ForeignKey("chat_id")]
        public virtual Chatbox? Chatbox { get; set; }

        [Display(Name = "Sender ID")]
        public string send_id { get; set; }

        [Display(Name = "Sender")]
        public string? send_name { get; set; }

        [Display(Name = "Receiver ID")]
        public string receive_id { get; set; }

        [Display(Name = "Receiver")]
        public string receive_name { get; set; }

        [Required]
        [Display(Name = "Chat Content")]
        public string? chat_ctn { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime timestamp { get; set; }

        public string? send_userid { get; set; }
    }
}