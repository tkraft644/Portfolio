using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExperienceStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeroBackgroundUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    AboutBioTemplatePl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AboutBioTemplateEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    TitlePl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DescriptionPl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioChallenges_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SchoolPl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SchoolEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioEducation_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioExperience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioExperience_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioHobbies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    TextPl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioHobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioHobbies_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitlePl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DescriptionPl = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioProjects_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioSocialLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioSocialLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioSocialLinks_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioTechnologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProfileId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTechnologies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioTechnologies_PortfolioProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "PortfolioProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioExperienceResponsibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioExperienceId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    TextPl = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioExperienceResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioExperienceResponsibilities_PortfolioExperience_PortfolioExperienceId",
                        column: x => x.PortfolioExperienceId,
                        principalTable: "PortfolioExperience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioProjectTechnologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioProjectId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioProjectTechnologies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioProjectTechnologies_PortfolioProjects_PortfolioProjectId",
                        column: x => x.PortfolioProjectId,
                        principalTable: "PortfolioProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioChallenges_PortfolioProfileId_SortOrder",
                table: "PortfolioChallenges",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioChallenges_ProfileId",
                table: "PortfolioChallenges",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioEducation_PortfolioProfileId_SortOrder",
                table: "PortfolioEducation",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioEducation_ProfileId",
                table: "PortfolioEducation",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioExperience_PortfolioProfileId_SortOrder",
                table: "PortfolioExperience",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioExperience_ProfileId",
                table: "PortfolioExperience",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioExperienceResponsibilities_PortfolioExperienceId",
                table: "PortfolioExperienceResponsibilities",
                column: "PortfolioExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioHobbies_PortfolioProfileId_SortOrder",
                table: "PortfolioHobbies",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioHobbies_ProfileId",
                table: "PortfolioHobbies",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioProjects_PortfolioProfileId_SortOrder",
                table: "PortfolioProjects",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioProjects_ProfileId",
                table: "PortfolioProjects",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioProjectTechnologies_PortfolioProjectId",
                table: "PortfolioProjectTechnologies",
                column: "PortfolioProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSocialLinks_PortfolioProfileId_SortOrder",
                table: "PortfolioSocialLinks",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioSocialLinks_ProfileId",
                table: "PortfolioSocialLinks",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTechnologies_PortfolioProfileId_SortOrder",
                table: "PortfolioTechnologies",
                columns: new[] { "PortfolioProfileId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTechnologies_ProfileId",
                table: "PortfolioTechnologies",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioChallenges");

            migrationBuilder.DropTable(
                name: "PortfolioEducation");

            migrationBuilder.DropTable(
                name: "PortfolioExperienceResponsibilities");

            migrationBuilder.DropTable(
                name: "PortfolioHobbies");

            migrationBuilder.DropTable(
                name: "PortfolioProjectTechnologies");

            migrationBuilder.DropTable(
                name: "PortfolioSocialLinks");

            migrationBuilder.DropTable(
                name: "PortfolioTechnologies");

            migrationBuilder.DropTable(
                name: "PortfolioExperience");

            migrationBuilder.DropTable(
                name: "PortfolioProjects");

            migrationBuilder.DropTable(
                name: "PortfolioProfiles");
        }
    }
}
