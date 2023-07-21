using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldContentFeedbackAndAddTableFeedbackFlieUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "feedback",
                newName: "staff_content");

            migrationBuilder.AddColumn<string>(
                name: "service_content",
                table: "feedback",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "feedback_file_upload",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    feedbackid = table.Column<long>(name: "feedback_id", type: "bigint", nullable: false),
                    namefile = table.Column<string>(name: "name_file", type: "nvarchar(MAX)", nullable: false),
                    typefile = table.Column<string>(name: "type_file", type: "nvarchar(MAX)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedback_file_upload", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedback_file_upload");

            migrationBuilder.DropColumn(
                name: "service_content",
                table: "feedback");

            migrationBuilder.RenameColumn(
                name: "staff_content",
                table: "feedback",
                newName: "content");
        }
    }
}
