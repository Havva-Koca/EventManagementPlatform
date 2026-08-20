using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVenueAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Venues",
                newName: "Street");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Venues",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Venues");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Venues",
                newName: "Address");
        }
    }
}
