using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripBookingBE.Migrations
{
    /// <inheritdoc />
    public partial class RowVersionForReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CustomerReviewTrip",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CustomerReviewTrip");
        }
    }
}
