using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldBookingDetailIdTableFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "feedback",
                type: "bigInt",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "booking_detail_id",
                table: "feedback",
                type: "bigInt",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "booking_detail_id",
                table: "feedback");

            migrationBuilder.AlterColumn<long>(
                name: "customer_id",
                table: "feedback",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigInt");
        }
    }
}
