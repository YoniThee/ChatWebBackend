using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChatWeb.Models
{
    public class UserConnection
    {
        public string UserName { get; set; } = string.Empty;
        public string ChatRoom { get; set;} = string.Empty;
        [Key]
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString();

        public bool UpdatedLastMessage { get; set; } = false;

    }
}
