using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManiaDeLimpeza.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Customers_CostumerId",
                table: "Quotes");

            migrationBuilder.RenameColumn(
                name: "CostumerId",
                table: "Quotes",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_CostumerId",
                table: "Quotes",
                newName: "IX_Quotes_CustomerId");

            migrationBuilder.RenameColumn(
                name: "TotalValue",
                table: "QuoteItems",
                newName: "TotalPrice");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentConditions",
                table: "Quotes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Quotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Customers_CustomerId",
                table: "Quotes",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Customers_CustomerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Quotes");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Quotes",
                newName: "CostumerId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes",
                newName: "IX_Quotes_CostumerId");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "QuoteItems",
                newName: "TotalValue");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentConditions",
                table: "Quotes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Customers_CostumerId",
                table: "Quotes",
                column: "CostumerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
