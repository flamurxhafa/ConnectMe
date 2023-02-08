using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    /// <inheritdoc />
    public partial class AddedMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Userid1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Userid2 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    match = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_Matches_AspNetUsers_Userid1",
                        column: x => x.Userid1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matches_AspNetUsers_Userid2",
                        column: x => x.Userid2,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Userid1",
                table: "Matches",
                column: "Userid1");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Userid2",
                table: "Matches",
                column: "Userid2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
