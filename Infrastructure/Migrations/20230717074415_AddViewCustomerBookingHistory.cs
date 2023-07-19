using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddViewCustomerBookingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE VIEW View_CustomerBookingHistory AS" +
                " SELECT booking_detail.booking_id, booking.booking_date, booking.from_time, booking.to_time, booking.status, booking.customer_id," +
                " booking_detail.service_id, service.price,service.name as service_name" +
                " FROM booking INNER JOIN booking_detail" +
                " ON booking.id = booking_detail.booking_id" +
                " INNER JOIN service" +
                " ON booking_detail.service_id = service.Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW View_CustomerBookingHistory");
        }
    }
}
