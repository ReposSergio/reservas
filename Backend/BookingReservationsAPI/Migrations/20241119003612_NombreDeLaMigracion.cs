using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingReservationsAPI.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Bookings",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Bookings",
                newName: "ReservationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationDate",
                table: "Bookings",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Bookings",
                newName: "Status");
        }
    }
}
