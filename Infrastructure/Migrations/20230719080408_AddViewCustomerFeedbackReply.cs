using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddViewCustomerFeedbackReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER VIEW View_CustomerBookingHistory AS" +
               " SELECT booking_detail.booking_id, booking.booking_date, booking.from_time, booking.to_time, booking.status, booking.customer_id," +
               " booking_detail.service_id, service.price,service.name as service_name" +
               " FROM booking INNER JOIN booking_detail" +
               " ON booking.id = booking_detail.booking_id" +
               " INNER JOIN service" +
               " ON booking_detail.service_id = service.Id" +
               " WHERE booking.IsDeleted = 0 AND booking_detail.IsDeleted = 0");
            migrationBuilder.Sql("CREATE VIEW View_CustomerFeedbackReply AS" +
                " SELECT feedback.Id as feedback_id, feedback.customer_id, customer.customer_name, customer.phone_number,feedback.service_id," +
                " service.name as service_name, feedback.title as feedback_title, feedback.content as feedback_content," + 
                " feedback.reply_id, reply.title as reply_title, reply.content as reply_content, feedback.rating" + 
                " FROM feedback INNER JOIN customer" +
                " ON customer.Id = feedback.customer_id" +
                " LEFT JOIN reply ON reply.Id = feedback.reply_id" +
                " INNER JOIN service ON service.Id = feedback.service_id" + 
                " WHERE feedback.IsDeleted = 0 AND service.IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW View_CustomerFeedbackReply");
            migrationBuilder.Sql("ALTER VIEW View_CustomerBookingHistory AS" +
               " SELECT booking_detail.booking_id, booking.booking_date, booking.from_time, booking.to_time, booking.status, booking.customer_id," +
               " booking_detail.service_id, service.price,service.name as service_name" +
               " FROM booking INNER JOIN booking_detail" +
               " ON booking.id = booking_detail.booking_id" +
               " INNER JOIN service" +
               " ON booking_detail.service_id = service.Id");
        }
    }
}
