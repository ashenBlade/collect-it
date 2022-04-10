using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddDefaultImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NormalizedUserName", "UserName" },
                values: new object[] { "BestPhotoshoper", "BestPhotoshoper" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Path" },
                values: new object[] { "Мониторы с аниме", "/imagesFromDb/abstract-img.jpg" });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Name", "OwnerId", "Path", "UploadDate" },
                values: new object[,]
                {
                    { 2, "Птица зимородок", 1, "/imagesFromDb/bird-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 3, "Машина на дороге", 1, "/imagesFromDb/car-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 4, "Котенок на одеяле", 1, "/imagesFromDb/cat-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 5, "Стандартный американский дом", 1, "/imagesFromDb/house-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 6, "Осенний лес в природе", 1, "/imagesFromDb/nature-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 7, "Дети за партами в школе перед учителем", 1, "/imagesFromDb/school-img.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 8, "Кот смотрит в камеру на зеленом фоне", 1, "/imagesFromDb/cat-img-2.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 9, "Крутой кот в очках", 1, "/imagesFromDb/cat-img-3.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 10, "Белоснежный кот застыл в мяукающей позе", 1, "/imagesFromDb/cat-img-4.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 11, "Рыжий кот заснул на полу", 1, "/imagesFromDb/cat-img-5.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 12, "Спящий кот прикрывается лапой от солнца", 1, "/imagesFromDb/cat-img-6.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 13, "На стуле лежит кот", 1, "/imagesFromDb/cat-img-7.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 14, "Идущий по забору кот у причала", 1, "/imagesFromDb/cat-img-8.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 15, "Кот у елки сморит на лес", 1, "/imagesFromDb/cat-img-9.jpg", new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Images",
                column: "Id",
                values: new object[]
                {
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12,
                    13,
                    14,
                    15
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NormalizedUserName", "UserName" },
                values: new object[] { null, "asdf@mail.ru" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Path" },
                values: new object[] { "Первое изображение", "/imagesFromDb/avaSig.jpg" });
        }
    }
}
