using Microsoft.EntityFrameworkCore.Migrations;

namespace RiseUpAPI.Migrations;

public partial class AddUsersTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ZipCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                Street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                State = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_CPF",
            table: "Users",
            column: "CPF",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");
    }
} 