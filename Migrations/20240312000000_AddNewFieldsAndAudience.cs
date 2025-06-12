using Microsoft.EntityFrameworkCore.Migrations;

namespace RiseUpAPI.Migrations;

public partial class AddNewFieldsAndAudience : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "RemoteOrOnline",
            table: "Opportunities",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Duration",
            table: "Opportunities",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Logo",
            table: "Organizations",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "Activities",
            type: "text",
            nullable: true);

        migrationBuilder.RenameColumn(
            name: "Title",
            table: "Activities",
            newName: "Name");

        migrationBuilder.CreateTable(
            name: "Audiences",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Scope = table.Column<string>(type: "text", nullable: false),
                Regions = table.Column<string[]>(type: "text[]", nullable: true),
                OpportunityId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Audiences", x => x.Id);
                table.ForeignKey(
                    name: "FK_Audiences_Opportunities_OpportunityId",
                    column: x => x.OpportunityId,
                    principalTable: "Opportunities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Audiences_OpportunityId",
            table: "Audiences",
            column: "OpportunityId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Audiences");

        migrationBuilder.DropColumn(
            name: "RemoteOrOnline",
            table: "Opportunities");

        migrationBuilder.DropColumn(
            name: "Duration",
            table: "Opportunities");

        migrationBuilder.DropColumn(
            name: "Logo",
            table: "Organizations");

        migrationBuilder.DropColumn(
            name: "Category",
            table: "Activities");

        migrationBuilder.RenameColumn(
            name: "Name",
            table: "Activities",
            newName: "Title");
    }
} 