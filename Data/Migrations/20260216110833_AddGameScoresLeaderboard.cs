using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameScoresLeaderboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioGameScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioGameScores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioGameScores_CreatedAtUtc",
                table: "PortfolioGameScores",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioGameScores_Score",
                table: "PortfolioGameScores",
                column: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioGameScores");
        }
    }
}
