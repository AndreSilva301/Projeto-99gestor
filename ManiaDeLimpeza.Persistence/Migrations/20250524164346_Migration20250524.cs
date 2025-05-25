using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManiaDeLimpeza.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Migration20250524 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Quotes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Quotes");
        }
    }
}
