using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class DriverNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Driver",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Route",
                table: "Trip");
            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Driver",
                table: "Trip",
                column: "driverId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Route",
                table: "Trip",
                column: "routeId",
                principalTable: "Route",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Driver",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Route",
                table: "Trip");

        }
    }
}
