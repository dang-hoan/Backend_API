using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldTableBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_name",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "booking");

            migrationBuilder.AddColumn<long>(
                name: "customer_id",
                table: "booking",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "booking");

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "booking",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "booking",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");
        }
    }
}
