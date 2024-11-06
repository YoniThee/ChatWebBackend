using ChatWeb.DataService;
using ChatWeb.Models;
using ChatWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace ChatWeb.Hubs
{
    public class ChatHub : Hub, IUserService
    {
        private readonly ChatDbContext _dbContext;

        public ChatHub(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
            var admin = new UserPersonalInfo() { UserName = "admin", Password = "123" };
            if (!_dbContext.Users.Contains(admin))
                _dbContext.Users.Add(admin);
        }

        public async Task JoinSpecificRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            // Find or create the UserConnection in the database
            var existingConnection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.UserName == connection.UserName);
            if (existingConnection == null || existingConnection.ChatRoom != connection.ChatRoom)
            {
                // Find or create the ChatTeam in the database
                var existingTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.ChatRoom == connection.ChatRoom);
                if (existingTeam == null)
                {
                        _dbContext.Teams.Add(new ChatTeam { ChatRoom = connection.ChatRoom });
                        await _dbContext.SaveChangesAsync();    
                }
                _dbContext.Connections.Add(new UserConnection { 
                    UserName = connection.UserName ,
                    ChatRoom = connection.ChatRoom,
                    UpdatedLastMessage = true,
                    ConnectionId = Guid.NewGuid().ToString()
                });
            }
            await _dbContext.SaveChangesAsync();

            await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificRoom", "admin", $"{connection.UserName} has joined to {connection.ChatRoom}");
        }

        public async Task SendMessage(string username, string chatRoom, string message)
        {
            var connection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.UserName == username);

            if (connection != null)
            {
                if (message != string.Empty)
                {
                    // Find the ChatTeam for the message
                    var chatTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.ChatRoom == chatRoom);
                    if (chatTeam != null)
                    {
                        chatTeam.Messages.Add(new MessageUser {
                            Message = message,
                            UserName = connection.UserName,
                            messageId = Guid.NewGuid().ToString()
                        });
                        await _dbContext.SaveChangesAsync();

                        // Send message to all users in the chat room
                        await Clients.Groups(chatRoom).SendAsync("ReceiveSpecificMessage", connection.UserName, message);
                    }
                }

                // Update "UpdatedLastMessage" for all members (excluding sender)
                var members = await _dbContext.Connections
                    .Where(c => c.ChatRoom == chatRoom && c.UserName != connection.UserName)
                    .ToListAsync();
                foreach (var member in members)
                {
                    member.UpdatedLastMessage = false;
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddUser(UserPersonalInfo userInfo)
        {
            // Validate username
            if (_dbContext.Users.Any(u => u.UserName == userInfo.UserName))
            {
                await Clients.Caller.SendAsync("AddUserResult", false, "Username already exists!");
                return;
            }

            if (userInfo.UserName.Equals(string.Empty))
            {
                await Clients.Caller.SendAsync("AddUserResult", false, "username cannot be empty!");
                return;
            }

            var user = new UserPersonalInfo { UserName = userInfo.UserName, Password = userInfo.Password };
            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            await Clients.Caller.SendAsync("AddUserResult", true, user);
        }

        public async Task GetUser(UserPersonalInfo userInfo)
        {
            // Find user by username
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userInfo.UserName);
            if (user == null)
            {
                await Clients.Caller.SendAsync("GetUserResult", false, "Username do not exists!");
                return;
            }

            if (!_dbContext.Users.Any(u => u.Password == userInfo.Password))
            {
                await Clients.Caller.SendAsync("GetUserResult", false, "Wrong password!");
                return;
            }
            await Clients.Caller.SendAsync("GetUserResult", user);
        }
        public async Task GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            await Clients.Caller.SendAsync("GetUsersList", users);
        }

        public async Task GetMessagesByTeam(string chatRoom)
        {
            try
            {
                var chatTeam = await _dbContext.Teams
                    .Include(t => t.Messages)
                    .FirstOrDefaultAsync(t => t.ChatRoom == chatRoom);

                if (chatTeam != null)
                {
                    await Clients.Caller.SendAsync("GetMessagesByChatRoom", chatTeam.Messages);
                }
                else
                {
                    await Clients.Caller.SendAsync("GetMessagesByChatRoom", new List<MessageUser>());
                }
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("GetMessagesByChatRoom", new List<MessageUser>());
            }
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                var connectionsToRemove = _dbContext.Connections.Where(c => c.UserName == userId);
                _dbContext.Connections.RemoveRange(connectionsToRemove);
                await _dbContext.SaveChangesAsync();

                await Clients.All.SendAsync("UserDeleted", true, userId);
            }
            else
            {
                await Clients.All.SendAsync("UserDeleted", false, userId);
            }
        }

        public async Task BlueV(string chatRoom)
        {
            var members = await _dbContext.Connections
                .Where(c => c.ChatRoom == chatRoom)
                .ToListAsync();

            bool hasUnreadMessages = members.Any(m => !m.UpdatedLastMessage);

            await Clients.All.SendAsync("GetBlueV", hasUnreadMessages);
        }
    }
}
