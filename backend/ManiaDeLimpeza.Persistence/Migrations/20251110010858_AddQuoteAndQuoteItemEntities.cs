using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManiaDeLimpeza.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteAndQuoteItemEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Customers_CostumerId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "QuoteItems");

            migrationBuilder.RenameColumn(
                name: "CostumerId",
                table: "Quotes",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_CostumerId",
                table: "Quotes",
                newName: "IX_Quotes_CustomerId");

            migrationBuilder.RenameColumn(
                name: "ExtraFields",
                table: "QuoteItems",
                newName: "CustomFields");

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

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QuoteItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "QuoteItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CreatedAt",
                table: "Quotes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteItems_Order",
                table: "QuoteItems",
                column: "Order");

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

            migrationBuilder.DropIndex(
                name: "IX_Quotes_CreatedAt",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_QuoteItems_Order",
                table: "QuoteItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "QuoteItems");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "QuoteItems");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Quotes",
                newName: "CostumerId");

            migrationBuilder.RenameIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes",
                newName: "IX_Quotes_CostumerId");

            migrationBuilder.RenameColumn(
                name: "CustomFields",
                table: "QuoteItems",
                newName: "ExtraFields");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentConditions",
                table: "Quotes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "QuoteItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
