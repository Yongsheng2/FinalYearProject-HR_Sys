namespace FinalYearProject.Models.ViewModels
{
    public class ChatboxVM
    {
        public Admin owner { get; set; }
        public EmployeeDetails owners { get; set; }
        public IEnumerable<Chatbox> ChatCreated { get; set; }
        public IEnumerable<ChatMessage> ChatMessagesCreated { get; set; }
        public Chatbox displayChatbox { get; set; } = new Chatbox();
        public ChatMessage displayChatMessages { get; set; } = new ChatMessage();
    }
}
