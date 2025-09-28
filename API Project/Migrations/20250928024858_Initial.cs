using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_Project.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jokes");

            migrationBuilder.CreateTable(
                name: "Audiences",
                schema: "jokes",
                columns: table => new
                {
                    AudienceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiences", x => x.AudienceId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "jokes",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Jokes",
                schema: "jokes",
                columns: table => new
                {
                    JokeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jokes", x => x.JokeId);
                    table.ForeignKey(
                        name: "FK_Jokes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "jokes",
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbAudienceDbJoke",
                schema: "jokes",
                columns: table => new
                {
                    AudiencesAudienceId = table.Column<int>(type: "integer", nullable: false),
                    JokesJokeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbAudienceDbJoke", x => new { x.AudiencesAudienceId, x.JokesJokeId });
                    table.ForeignKey(
                        name: "FK_DbAudienceDbJoke_Audiences_AudiencesAudienceId",
                        column: x => x.AudiencesAudienceId,
                        principalSchema: "jokes",
                        principalTable: "Audiences",
                        principalColumn: "AudienceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbAudienceDbJoke_Jokes_JokesJokeId",
                        column: x => x.JokesJokeId,
                        principalSchema: "jokes",
                        principalTable: "Jokes",
                        principalColumn: "JokeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbAudienceDbJoke_JokesJokeId",
                schema: "jokes",
                table: "DbAudienceDbJoke",
                column: "JokesJokeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jokes_CategoryId",
                schema: "jokes",
                table: "Jokes",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbAudienceDbJoke",
                schema: "jokes");

            migrationBuilder.DropTable(
                name: "Audiences",
                schema: "jokes");

            migrationBuilder.DropTable(
                name: "Jokes",
                schema: "jokes");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "jokes");
        }
    }
}
