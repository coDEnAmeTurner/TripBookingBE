using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralParam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "generalParamId",
                table: "Ticket",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GeneralParam",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paramKey = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    paramCode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    paramDescription = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralParam_1", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_generalParamId",
                table: "Ticket",
                column: "generalParamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_GeneralParam",
                table: "Ticket",
                column: "generalParamId",
                principalTable: "GeneralParam",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_GeneralParam",
                table: "Ticket");

            migrationBuilder.DropTable(
                name: "GeneralParam");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_generalParamId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "generalParamId",
                table: "Ticket");
        }
    }
}
