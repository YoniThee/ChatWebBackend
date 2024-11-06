using ChatWeb.Models;
using System.Collections.Concurrent;
namespace ChatWeb.DataService
{
    public class SharedDb 
    {

        private readonly ChatDbContext _dbContext;

        public SharedDb(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
