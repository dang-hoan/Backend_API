using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldTableFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "feedback");

            migrationBuilder.DropColumn(
                name: "name",
                table: "feedback");

            migrationBuilder.AddColumn<long>(
                name: "customer_id",
                table: "feedback",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "feedback");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "feedback",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "feedback",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }
    }
}
