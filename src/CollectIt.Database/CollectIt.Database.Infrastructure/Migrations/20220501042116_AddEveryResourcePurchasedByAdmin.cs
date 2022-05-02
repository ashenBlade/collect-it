using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddEveryResourcePurchasedByAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AcquiredUserResources",
                columns: new[] { "Id", "AcquiredDate", "ResourceId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 20, 1 },
                    { 2, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 21, 1 },
                    { 3, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 16, 1 },
                    { 4, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 17, 1 },
                    { 5, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 18, 1 },
                    { 6, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 19, 1 },
                    { 7, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 1, 1 },
                    { 8, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 2, 1 },
                    { 9, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 3, 1 },
                    { 10, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 4, 1 },
                    { 11, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 5, 1 },
                    { 12, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 6, 1 },
                    { 13, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 7, 1 },
                    { 14, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 8, 1 },
                    { 15, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 9, 1 },
                    { 16, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 10, 1 },
                    { 17, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 11, 1 },
                    { 18, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 12, 1 },
                    { 19, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 13, 1 },
                    { 20, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 14, 1 },
                    { 21, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc), 15, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AcquiredUserResources",
                keyColumn: "Id",
                keyValue: 21);
        }
    }
}
