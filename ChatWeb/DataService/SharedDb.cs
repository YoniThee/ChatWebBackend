using ChatWeb.Models;
using System.Collections.Concurrent;
namespace ChatWeb.DataService
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connections = new ConcurrentDictionary<string, UserConnection>();
        private readonly ConcurrentDictionary<string, UserPersonalInfo> _users = new ConcurrentDictionary<string, UserPersonalInfo>();
        private readonly List<ChatTeam> _teams = new List<ChatTeam>();
        public ConcurrentDictionary<string, UserConnection> Connections => _connections;
        public ConcurrentDictionary<string, UserPersonalInfo> Users => _users;
        public List<ChatTeam> Teams => _teams;

    }
}
