using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewReservationApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSubjecttoDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Reservations",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Reservations",
                newName: "Subject");
        }
    }
}
