using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class DefineSellerTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.CreateTable(
                name: "SellerTrip",
                columns: table => new
                {
                    SellersId = table.Column<long>(type: "bigint", nullable: false),
                    SellingTripsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerTrip", x => new { x.SellersId, x.SellingTripsId });
                    table.ForeignKey(
                        name: "FK_SellerTrip_Trip_SellingTripsId",
                        column: x => x.SellingTripsId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerTrip_User_SellersId",
                        column: x => x.SellersId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerTrip_SellingTripsId",
                table: "SellerTrip",
                column: "SellingTripsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip",
                column: "tripId",
                principalTable: "Trip",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip");

            migrationBuilder.DropTable(
                name: "SellerTrip");


            migrationBuilder.AddForeignKey(
                name: "FK_UserReviewTrip_Trip",
                table: "CustomerReviewTrip",
                column: "tripId",
                principalTable: "Trip",
                principalColumn: "id");
        }
    }
}
