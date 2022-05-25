using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddActiveSubscriptionForDefaultUserOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 5,
                column: "MonthDuration",
                value: 120000);

            migrationBuilder.InsertData(
                table: "UsersSubscriptions",
                columns: new[] { "Id", "During", "LeftResourcesCount", "SubscriptionId", "UserId" },
                values: new object[] { 5, new NodaTime.DateInterval(new NodaTime.LocalDate(2022, 5, 1), new NodaTime.LocalDate(2022, 7, 1)), 200, 3, 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersSubscriptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 5,
                column: "MonthDuration",
                value: 2147483647);
        }
    }
}
