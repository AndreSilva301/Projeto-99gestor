using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManiaDeLimpeza.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToCustomerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRelationships_Customers_CostumerId",
                table: "CustomerRelationships");

            migrationBuilder.DropColumn(
                name: "DateTimeCustomer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomerRelationships");

            migrationBuilder.RenameColumn(
                name: "CostumerId",
                table: "CustomerRelationships",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerRelationships_CostumerId",
                table: "CustomerRelationships",
                newName: "IX_CustomerRelationships_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRelationships_Customers_CustomerId",
                table: "CustomerRelationships",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRelationships_Customers_CustomerId",
                table: "CustomerRelationships");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "CustomerRelationships",
                newName: "CostumerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerRelationships_CustomerId",
                table: "CustomerRelationships",
                newName: "IX_CustomerRelationships_CostumerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeCustomer",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomerRelationships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRelationships_Customers_CostumerId",
                table: "CustomerRelationships",
                column: "CostumerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
