using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mille.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToProductImageAndRemoveVariantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductVariantId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ProductImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_PublicId",
                table: "ProductImages",
                column: "PublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_PublicId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductImages");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariantId",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductVariantId",
                table: "ProductImages",
                column: "ProductVariantId");
        }
    }
}
