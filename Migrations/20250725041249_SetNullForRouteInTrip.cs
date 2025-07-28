using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class SetNullForRouteInTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Route",
                table: "Trip");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Route",
                table: "Trip",
                column: "routeId",
                principalTable: "Route",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Route",
                table: "Trip");

            migrationBuilder.DropTable(
                name: "TripUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Route",
                table: "Trip",
                column: "routeId",
                principalTable: "Route",
                principalColumn: "id");
        }
    }
}
