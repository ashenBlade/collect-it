using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class addbasedvideosandmusics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Extension", "FileName", "Name", "OwnerId", "Tags", "UploadDate" },
                values: new object[,]
                {
                    { 16, "mp3", "тектоник-басы.mp3", "Тектоник - Басы", 1, new[] { "качает", "2007" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 17, "mp3", "MORGENSHTERN_JESTKO_VALIT.mp3", "OG BUDA, MORGENSHTERN, Mayot, blago white, SODA LUV - Cristal & МОЁТ (Remix)", 1, new[] { "качает", "морген", "сода лув", "ог буда" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 18, "mp3", "naruto_bluebird.mp3", "OST Naruto shippuden Ikimono-gakari - Blue Bird OP3", 1, new[] { "аниме", "наруто", "афган" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 19, "mp3", "minin_zeleniy_glaz.mp3", "минин - Зелёный глаз", 1, new[] { "грусть", "тикток", "рэп про тёлку" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 20, "webm", "diman.webm", "Диско лицо", 1, new[] { "Брекоткин", "диско лицо", "диско" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) },
                    { 21, "webm", "strong_monolog.webm", "Сильный монолог на фоне церковных песнопений и красивой картинки", 1, new[] { "аниме", "церковь", "2д", "монолог" }, new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Musics",
                columns: new[] { "Id", "Duration" },
                values: new object[,]
                {
                    { 16, 69 },
                    { 17, 219 },
                    { 18, 218 },
                    { 19, 114 }
                });

            migrationBuilder.InsertData(
                table: "Videos",
                columns: new[] { "Id", "Duration" },
                values: new object[,]
                {
                    { 20, 60 },
                    { 21, 60 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Musics",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Musics",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Musics",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Musics",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Videos",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Videos",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 21);
        }
    }
}
