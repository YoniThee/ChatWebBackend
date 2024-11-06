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
/*{
    public class ChatHub : Hub, IUserService
    {

        private readonly SharedDb _sharedDb;
        public ChatHub(SharedDb sharedDb) {
            _sharedDb = sharedDb;
            _sharedDb.Users.TryAdd("stringToConnectAdmin",new UserPersonalInfo {UserName = "admin", Password = "123" });
        }


        public async Task JoinSpecificRoom(UserConnection connection) {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatTeam.ChatRoom);

            _sharedDb.Connections[Context.ConnectionId] = connection;
            var exist = _sharedDb.Teams.Find(x => x.ChatRoom == connection.ChatTeam.ChatRoom);
            if(exist?.ChatRoom != connection.ChatTeam.ChatRoom)
                _sharedDb.Teams.Add(connection.ChatTeam);
            _sharedDb.Connections.FirstOrDefault(x => x.Value.UserName == connection.UserName).Value.UpdatedLastMessage = true;
            await Clients.Group(connection.ChatTeam.ChatRoom).SendAsync("JoinSpecificRoom", "admin",$"{connection.UserName} has joined to {connection.ChatTeam}");
        }
        
        public async Task SendMessage(string chatRoom, string message)
        {
            if (_sharedDb.Connections.TryGetValue(Context.ConnectionId, out UserConnection connection))
            {
                if (message != string.Empty)
                {
                    _sharedDb.Teams.Find(c => c.ChatRoom == chatRoom).Messages.Add(new MessageUser() { Message = message, UserName = connection.UserName });
                    await Clients.Groups(chatRoom).SendAsync("ReceiveSpecificMessage", connection.UserName, message);             
                }

                //After message is sended only member how will join the team after this message will see the message
                var membersInChat = _sharedDb.Connections.FirstOrDefault(u => u.Value.UserName == connection.UserName).Value.ChatTeam.GetMembers();

                foreach (var member in membersInChat) {
                    if (member == connection.UserName) {
                        _sharedDb.Connections.FirstOrDefault(x => x.Value.UserName == connection.UserName).Value.UpdatedLastMessage = true;
                    }
                    //All the other members in this chatTeam
                    else {
                        _sharedDb.Connections.FirstOrDefault(x => x.Value.UserName != connection.UserName).Value.UpdatedLastMessage = false;
                    }
                }
            }
        }

        public async Task AddUser(UserPersonalInfo userInfo)
        {
            // Validate username
            if (_sharedDb.Users.Any(u => u.Value.UserName == userInfo.UserName))
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
            _sharedDb.Users.TryAdd(Context.ConnectionId,user);
            _sharedDb.Connections.TryAdd(Context.ConnectionId, new UserConnection() {UserName = user.UserName });

            await Clients.Caller.SendAsync("AddUserResult", true, user);
        }

        public async Task GetUser(UserPersonalInfo userInfo)
        {
            // Find user by username
            var user = _sharedDb.Users.FirstOrDefault(u => u.Value.UserName == userInfo.UserName);
            if (user.Value == null)
            {
                await Clients.Caller.SendAsync("GetUserResult", false, "Username do not exists!");
                return;
            }
            if (_sharedDb.Users.FirstOrDefault(u => u.Value.Password == userInfo.Password).Value == null) {
                await Clients.Caller.SendAsync("GetUserResult", false, "Wrong password!");
                return;
            }

            // Send userConnection (with chats) object to client
            await Clients.Caller.SendAsync("GetUserResult", user);
        }

        public async Task GetAllUsers()
        {
            var users = _sharedDb.Users.Values.ToList();
            await Clients.Caller.SendAsync("GetUsersList", users);
        }

        public async Task GetMessagesByTeam(string chatRoom)
        {
            try
            {
                var messages = _sharedDb.Teams.Find(team => team.ChatRoom == chatRoom).Messages;
                await Clients.Caller.SendAsync("GetMessagesByChatRoom", messages);
            }
            catch (Exception e){
                await Clients.Caller.SendAsync("GetMessagesByChatRoom", new List<MessageUser>());
            }
        }

        public async Task DeleteUser(string userId)
        {
            var connectionIdToRemove = _sharedDb.Users.FirstOrDefault(u => u.Value.UserName == userId).Key;
            if (!string.IsNullOrEmpty(connectionIdToRemove))
            {
                if (_sharedDb.Users.TryRemove(connectionIdToRemove, out _))
                {
                    await Clients.All.SendAsync("UserDeleted", true, userId);
                }
                else
                {
                    await Clients.All.SendAsync("UserDeleted", false, userId);
                }
            }
            else
            {
                await Clients.All.SendAsync("UserDeleted", false, userId);
            }
        }

        public async Task BlueV(string chatRoom) {
            foreach (var user in _sharedDb.Connections) {
                if(user.Value.UpdatedLastMessage == false)
                    await Clients.All.SendAsync("GetBlueV", false);
            }
            await Clients.All.SendAsync("GetBlueV", true);
        }

    }
}*/
