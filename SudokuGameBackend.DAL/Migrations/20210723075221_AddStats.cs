using Microsoft.EntityFrameworkCore.Migrations;

namespace SudokuGameBackend.DAL.Migrations
{
    public partial class AddStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuelStats",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    GamesStarted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    GamesWon = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuelStats", x => new { x.UserId, x.GameMode });
                    table.ForeignKey(
                        name: "FK_DuelStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleStats",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    GamesStarted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleStats", x => new { x.UserId, x.GameMode });
                    table.ForeignKey(
                        name: "FK_SingleStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuelStats");

            migrationBuilder.DropTable(
                name: "SingleStats");
        }
    }
}
