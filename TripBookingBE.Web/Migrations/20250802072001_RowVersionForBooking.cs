using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class RowVersionForBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CustomerBookTrip",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CustomerBookTrip");
        }
    }
}
