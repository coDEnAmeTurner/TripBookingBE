using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Commons.Migrations
{
    /// <inheritdoc />
    public partial class TicketColPaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "paid",
                table: "Ticket",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paid",
                table: "Ticket");
        }
    }
}
