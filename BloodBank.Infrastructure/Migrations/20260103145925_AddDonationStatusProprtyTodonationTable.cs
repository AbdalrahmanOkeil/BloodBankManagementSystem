using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationStatusProprtyTodonationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Donations",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Donations");

        }
    }
}
