﻿// <auto-generated />
using ChatWeb.DataService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatWeb.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20241105180038_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatWeb.Models.ChatTeam", b =>
                {
                    b.Property<string>("ChatRoom")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ChatRoom");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("ChatWeb.Models.MessageUser", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChatTeamChatRoom")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserName");

                    b.HasIndex("ChatTeamChatRoom");

                    b.ToTable("MessageUser");
                });

            modelBuilder.Entity("ChatWeb.Models.UserConnection", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ChatTeamChatRoom")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("UpdatedLastMessage")
                        .HasColumnType("bit");

                    b.HasKey("UserName");

                    b.HasIndex("ChatTeamChatRoom");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("ChatWeb.Models.UserPersonalInfo", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatWeb.Models.MessageUser", b =>
                {
                    b.HasOne("ChatWeb.Models.ChatTeam", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChatTeamChatRoom");
                });

            modelBuilder.Entity("ChatWeb.Models.UserConnection", b =>
                {
                    b.HasOne("ChatWeb.Models.ChatTeam", "ChatTeam")
                        .WithMany()
                        .HasForeignKey("ChatTeamChatRoom")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatTeam");
                });

            modelBuilder.Entity("ChatWeb.Models.ChatTeam", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
