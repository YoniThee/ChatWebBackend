using ChatWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWeb.DataService
{
    public class ChatDbContext : DbContext
    {

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        public DbSet<UserConnection> Connections { get; set; }
        public DbSet<UserPersonalInfo> Users { get; set; }
        public DbSet<ChatTeam> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-6PQJSFF;Database=myChatApp;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatTeam>()
                .HasKey(c => c.ChatRoom);
            modelBuilder.Entity<UserPersonalInfo>()
                .HasKey(c => c.UserName);
            modelBuilder.Entity<UserConnection>()
                .HasKey(c => c.ConnectionId);
            modelBuilder.Entity<MessageUser>()
           .HasKey(mu => mu.messageId);
        }
    }
}
