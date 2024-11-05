namespace ChatWeb.Models
{
    public class ChatTeam
    {
        public string ChatRoom { get; set;} = string.Empty;
        public List<MessageUser> Messages { get; set; } = new List<MessageUser>();

        public List<string> GetMembers()
        {
            return Messages
                .GroupBy(m => m.UserName)
                .Select(g => g.First().UserName)
                .ToList();
        }
           



    }
    public class MessageUser {
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
