namespace ChatWeb.Models
{
    public class UserConnection
    {
        public string UserName { get; set; } = string.Empty;
        public ChatTeam ChatTeam { get; set;} = new ChatTeam();
        public bool UpdatedLastMessage { get; set; } = false;

    }
}
