using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddMoreInitialAccountEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 2, 0, "31ab9dd7-d86c-4640-aa97-22ff38176d94", "mail@mail.ru", false, false, null, "MAIL@MAIL.RU", "BESTPHOTOSHOPER", "AQAAAAEAACcQAAAAEENZCDY7KW1yCVxiLaIjILavAHSPVWMvTeb0YjDdOK74mqCBqby19ul9VfFQk6Il9A==", null, false, null, "TX26HJDK44UKB7FQTM3WSW7A5K4PRRS6", false, "Discriminator" },
                    { 3, 0, "f1a6e983-61f0-4fe3-b201-e8131080d312", "andrey1999@yandex.ru", false, false, null, "ANDREY1999@YANDEX.RU", "ANDREYPHOTO", "AQAAAAEAACcQAAAAEDFG3rJjU9RopPeh1w+EePG21c/o6h2ng8hgRiQactvUbYOKSeLjxL/HAhJfDsuO0A==", null, false, null, "AG44W4JZWJVREA7HQRCKUFDSNZDYKCAW", false, "AndreyPhoto" },
                    { 4, 0, "fac5fa96-0453-4eaf-bebb-bc7ad73299d2", "user@mail.ru", false, false, null, "USER@MAIL.RU", "NINEONEONE", "AQAAAAEAACcQAAAAEO63OCfJlqJdesMS4+ORyynU0r6Y/3x8u0j9ZQsd52y6ELqZG0f1X/WN49PV2NQWkA==", null, false, null, "A7NZSQXBUSPXKD4PTF5DPC3LTROWH2PH", false, "NineOneOne" }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "Active", "AppliedResourceType", "Description", "MaxResourcesCount", "MonthDuration", "Name", "Price", "RestrictionId" },
                values: new object[] { 5, false, "Any", "Этот тип подписки только для привилегированных. Скачивай что хочешь.", 2147483647, 2147483647, "Кардбланш", 2147483647, null });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 3, 3 });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                column: "OwnerId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                column: "OwnerId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                column: "OwnerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                column: "OwnerId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                column: "OwnerId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                column: "OwnerId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                column: "OwnerId",
                value: 3);

            migrationBuilder.InsertData(
                table: "UsersSubscriptions",
                columns: new[] { "Id", "During", "LeftResourcesCount", "SubscriptionId", "UserId" },
                values: new object[,]
                {
                    { 1, new NodaTime.DateInterval(new NodaTime.LocalDate(2000, 1, 1), new NodaTime.LocalDate(3000, 1, 1)), 2147483647, 5, 1 },
                    { 2, new NodaTime.DateInterval(new NodaTime.LocalDate(2021, 3, 1), new NodaTime.LocalDate(2021, 5, 9)), 0, 2, 3 },
                    { 3, new NodaTime.DateInterval(new NodaTime.LocalDate(2021, 5, 10), new NodaTime.LocalDate(2022, 1, 10)), 2, 3, 3 },
                    { 4, new NodaTime.DateInterval(new NodaTime.LocalDate(2022, 2, 20), new NodaTime.LocalDate(2022, 5, 20)), 50, 1, 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UsersSubscriptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UsersSubscriptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UsersSubscriptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UsersSubscriptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                column: "OwnerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                column: "OwnerId",
                value: 1);
        }
    }
}
