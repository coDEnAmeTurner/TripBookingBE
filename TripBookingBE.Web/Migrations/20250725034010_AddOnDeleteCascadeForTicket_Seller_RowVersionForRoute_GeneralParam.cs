using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class AddOnDeleteCascadeForTicket_Seller_RowVersionForRoute_GeneralParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBookTrip_User",
                table: "CustomerBookTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "User",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Route",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBookTrip_Customer",
                table: "CustomerBookTrip",
                column: "customerId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip",
                column: "tripId",
                principalTable: "Trip",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Driver",
                table: "Trip",
                column: "driverId",
                principalTable: "User",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBookTrip_Customer",
                table: "CustomerBookTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Driver",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_User",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Route");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBookTrip_User",
                table: "CustomerBookTrip",
                column: "customerId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip",
                column: "tripId",
                principalTable: "Trip",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
