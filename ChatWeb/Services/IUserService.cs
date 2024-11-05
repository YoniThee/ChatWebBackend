using ChatWeb.Models;

namespace ChatWeb.Services
{
    public interface IUserService
    {
        Task AddUser(UserPersonalInfo userInfo);
        Task GetUser(UserPersonalInfo userInfo);
        Task GetAllUsers();
        Task DeleteUser(string userId);
    }
}
