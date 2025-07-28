using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    routeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    lastLogin = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    userName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    firstName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    lastName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "text", nullable: true, defaultValueSql: "(NULL)"),
                    active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    avatar = table.Column<string>(type: "text", nullable: true, defaultValueSql: "(NULL)"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValue: "CUSTOMER"),
                    sellerCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3213E83F36DAC7FF", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departureTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    placeCount = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    registrationNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    driverId = table.Column<long>(type: "bigint", nullable: true),
                    routeId = table.Column<long>(type: "bigint", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.id);
                    table.ForeignKey(
                        name: "FK_Trip_Route",
                        column: x => x.routeId,
                        principalTable: "Route",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Trip_User",
                        column: x => x.driverId,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerBookTrip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<long>(type: "bigint", nullable: false),
                    tripId = table.Column<long>(type: "bigint", nullable: false),
                    placeNumber = table.Column<int>(type: "int", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBookTrip_1", x => x.id);
                    table.ForeignKey(
                        name: "FK_CustomerBookTrip_Trip",
                        column: x => x.tripId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerBookTrip_User",
                        column: x => x.customerId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviewTrip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<long>(type: "bigint", nullable: false),
                    tripId = table.Column<long>(type: "bigint", nullable: true),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviewTrip", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserReviewTrip_Trip",
                        column: x => x.tripId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviewTrip_User",
                        column: x => x.customerId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerTrip",
                columns: table => new
                {
                    sellerId = table.Column<long>(type: "bigint", nullable: false),
                    tripId = table.Column<long>(type: "bigint", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerTrip", x => new { x.sellerId, x.tripId });
                    table.ForeignKey(
                        name: "FK_SellerTrip_Trip",
                        column: x => x.tripId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    customerBookTripId = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: true),
                    sellerCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.customerBookTripId);
                    table.ForeignKey(
                        name: "FK_Ticket_CustomerBookTrip",
                        column: x => x.customerBookTripId,
                        principalTable: "CustomerBookTrip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBookTrip",
                table: "CustomerBookTrip",
                columns: new[] { "customerId", "tripId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBookTrip_tripId",
                table: "CustomerBookTrip",
                column: "tripId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviewTrip_customerId",
                table: "CustomerReviewTrip",
                column: "customerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviewTrip_tripId",
                table: "CustomerReviewTrip",
                column: "tripId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerTrip_tripId",
                table: "SellerTrip",
                column: "tripId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_driverId",
                table: "Trip",
                column: "driverId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_routeId",
                table: "Trip",
                column: "routeId");

            migrationBuilder.CreateIndex(
                name: "UQ__User__66DCF95C55A682B1",
                table: "User",
                column: "userName",
                unique: true);
        }

        // / <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReviewTrip");

            migrationBuilder.DropTable(
                name: "SellerTrip");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "CustomerBookTrip");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
