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

            migrationBuilder.CreateTable(
                name: "TripUser",
                columns: table => new
                {
                    CustomerTripsId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripUser", x => new { x.CustomerTripsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TripUser_Trip_CustomerTripsId",
                        column: x => x.CustomerTripsId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripUser_UsersId",
                table: "TripUser",
                column: "UsersId");

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
