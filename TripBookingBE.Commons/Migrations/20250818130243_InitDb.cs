using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TripBookingBE.Commons.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GeneralParams",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    paramKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    paramCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paramDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 2000, nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralParam_1", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    routeDescription = table.Column<string>(type: "longtext", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    password = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    lastLogin = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    userName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    firstName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    lastName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "text", nullable: true, defaultValueSql: "(NULL)"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    avatar = table.Column<string>(type: "text", nullable: true, defaultValueSql: "(NULL)"),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValue: "CUSTOMER"),
                    sellerCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "longtext", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "longtext", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3213E83F36DAC7FF", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    departureTime = table.Column<DateTime>(type: "datetime", nullable: true),
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
                        name: "FK_Trip_Driver",
                        column: x => x.driverId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Trip_Route",
                        column: x => x.routeId,
                        principalTable: "Route",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerBookTrip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    customerId = table.Column<long>(type: "bigint", nullable: true),
                    tripId = table.Column<long>(type: "bigint", nullable: true),
                    placeNumber = table.Column<int>(type: "int", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    dateModified = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBookTrip_1", x => x.id);
                    table.ForeignKey(
                        name: "FK_CustomerBookTrip_Customer",
                        column: x => x.customerId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerBookTrip_Trip",
                        column: x => x.tripId,
                        principalTable: "Trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerReviewTrip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    customerId = table.Column<long>(type: "bigint", nullable: true),
                    tripId = table.Column<long>(type: "bigint", nullable: true),
                    content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
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
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    customerBookTripId = table.Column<long>(type: "bigint", nullable: false),
                    generalParamId = table.Column<long>(type: "bigint", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Ticket_GeneralParam",
                        column: x => x.generalParamId,
                        principalTable: "GeneralParams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                name: "IX_SellerTrip_SellingTripsId",
                table: "SellerTrip",
                column: "SellingTripsId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_generalParamId",
                table: "Ticket",
                column: "generalParamId");

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

        /// <inheritdoc />
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
                name: "GeneralParams");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Route");
        }
    }
}
