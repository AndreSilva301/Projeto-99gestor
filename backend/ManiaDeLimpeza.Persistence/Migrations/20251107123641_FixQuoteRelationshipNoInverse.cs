using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManiaDeLimpeza.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixQuoteRelationshipNoInverse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QuoteItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuoteId1",
                table: "QuoteItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QuoteItems_QuoteId1",
                table: "QuoteItems",
                column: "QuoteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteItems_Quotes_QuoteId1",
                table: "QuoteItems",
                column: "QuoteId1",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuoteItems_Quotes_QuoteId1",
                table: "QuoteItems");

            migrationBuilder.DropIndex(
                name: "IX_QuoteItems_QuoteId1",
                table: "QuoteItems");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "QuoteItems");

            migrationBuilder.DropColumn(
                name: "QuoteId1",
                table: "QuoteItems");
        }
    }
}
