using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewReservationApi.Migrations
{
    /// <inheritdoc />
    public partial class fullname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "Driver",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "Driver");
        }
    }
}
