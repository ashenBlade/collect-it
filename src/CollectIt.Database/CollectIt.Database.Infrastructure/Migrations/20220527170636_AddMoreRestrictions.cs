using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddMoreRestrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Restriction",
                columns: new[] { "Id", "AuthorId", "RestrictionType" },
                values: new object[] { 2, 1, 5 });

            migrationBuilder.InsertData(
                table: "Restriction",
                columns: new[] { "Id", "RestrictionType", "Tags" },
                values: new object[] { 1, 6, new[] { "видео" } });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "Active", "AppliedResourceType", "Description", "MaxResourcesCount", "MonthDuration", "Name", "Price", "RestrictionId" },
                values: new object[,]
                {
                    { 6, true, "Video", "Только видео с тегом \"видео\"", 100, 10, "Хочю видео", 600, 1 },
                    { 7, true, "Music", "Только музыка Админа \"BestPhotoshoper\"", 1000, 3, "Люблю админа", 228, 2 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Restriction",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Restriction",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
