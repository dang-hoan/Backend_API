using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddViewCustomerReviewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER VIEW View_CustomerFeedbackReply AS" +
                " SELECT feedback.Id as feedback_id, feedback.customer_id, customer.customer_name, customer.phone_number,feedback.service_id," +
                " service.name as service_name, feedback.title as feedback_title, feedback.service_content as feedback_service_content," +
                " feedback.staff_content as feedback_staff_content,feedback.reply_id, reply.title as reply_title, reply.content as reply_content, feedback.rating" +
                " FROM feedback INNER JOIN customer" +
                " ON customer.Id = feedback.customer_id" +
                " LEFT JOIN reply ON reply.Id = feedback.reply_id" +
                " INNER JOIN service ON service.Id = feedback.service_id" +
                " WHERE feedback.IsDeleted = 0 AND service.IsDeleted = 0");
            migrationBuilder.Sql("CREATE VIEW View_CustomerReviewHistory AS" +
                " Select booking.id as booking_id, service.Id as service_id, service.name as service_name, customer.Id as customer_id, customer.customer_name," +
                " feedback.Id as feedback_id, feedback.title as feedback_title, feedback.staff_content as feedback_staff_content," +
                " feedback.service_content as feedback_service_content, feedback.rating, feedback.CreatedOn as create_on_feedback," +
                " feedback.reply_id" +
                " From booking" +
                " INNER JOIN booking_detail" +
                " ON booking.id = booking_detail.booking_id" +
                " INNER JOIN feedback" +
                " ON feedback.booking_detail_id = booking_detail.Id" +
                " INNER JOIN customer" +
                " ON customer.Id = feedback.customer_id" +
                " INNER JOIN service" +
                " ON service.Id = booking_detail.service_id" +
                " Where booking_detail.IsDeleted = 0 and booking.IsDeleted = 0 and feedback.IsDeleted = 0 and service.IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW View_CustomerReviewHistory");
            migrationBuilder.Sql("ALTER VIEW View_CustomerFeedbackReply AS" +
               " SELECT feedback.Id as feedback_id, feedback.customer_id, customer.customer_name, customer.phone_number,feedback.service_id," +
               " service.name as service_name, feedback.title as feedback_title, feedback.content as feedback_content," +
               " feedback.reply_id, reply.title as reply_title, reply.content as reply_content, feedback.rating" +
               " FROM feedback INNER JOIN customer" +
               " ON customer.Id = feedback.customer_id" +
               " LEFT JOIN reply ON reply.Id = feedback.reply_id" +
               " INNER JOIN service ON service.Id = feedback.service_id" +
               " WHERE feedback.IsDeleted = 0 AND service.IsDeleted = 0");
        }
    }
}
