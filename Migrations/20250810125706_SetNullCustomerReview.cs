using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class SetNullCustomerReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_User",
                table: "CustomerReviewTrip");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip",
                column: "tripId",
                principalTable: "Trip",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_User",
                table: "CustomerReviewTrip",
                column: "customerId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_User",
                table: "CustomerReviewTrip");

        }
    }
}
