using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ChatRoom = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ChatRoom);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatTeamChatRoom = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedLastMessage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_Connections_Teams_ChatTeamChatRoom",
                        column: x => x.ChatTeamChatRoom,
                        principalTable: "Teams",
                        principalColumn: "ChatRoom",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageUser",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChatTeamChatRoom = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUser", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_MessageUser_Teams_ChatTeamChatRoom",
                        column: x => x.ChatTeamChatRoom,
                        principalTable: "Teams",
                        principalColumn: "ChatRoom");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_ChatTeamChatRoom",
                table: "Connections",
                column: "ChatTeamChatRoom");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUser_ChatTeamChatRoom",
                table: "MessageUser",
                column: "ChatTeamChatRoom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "MessageUser");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
