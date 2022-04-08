using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class RussianTagsInsteadEnglish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                column: "Tags",
                value: new[] { "аниме", "фоллаут" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2,
                column: "Tags",
                value: new[] { "птица", "природа" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                column: "Tags",
                value: new[] { "машина" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4,
                column: "Tags",
                value: new[] { "кот", "животное", "питомец" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                column: "Tags",
                value: new[] { "дом" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                column: "Tags",
                value: new[] { "природа" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 7,
                column: "Tags",
                value: new[] { "школа", "дети" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 9,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное", "очки" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 10,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 13,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное", "стул", "мебель" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 14,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное", "яхта", "море" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                column: "Tags",
                value: new[] { "кот", "питомец", "животное", "природа" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                                        table: "Resources",
                                        keyColumn: "Id",
                                        keyValue: 1,
                                        column: "Tags",
                                        value: new[] { "anime", "fallout" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2,
                column: "Tags",
                value: new[] { "bird", "nature" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                column: "Tags",
                value: new[] { "car" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4,
                column: "Tags",
                value: new[] { "cat", "animal", "pet" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                column: "Tags",
                value: new[] { "house" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                column: "Tags",
                value: new[] { "nature" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 7,
                column: "Tags",
                value: new[] { "school", "kids" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                column: "Tags",
                value: new[] { "cat", "pet", "animal" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 9,
                column: "Tags",
                value: new[] { "cat", "pet", "animal", "sunglasses" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 10,
                column: "Tags",
                value: new[] { "cat", "pet", "animal" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                column: "Tags",
                value: new[] { "cat", "pet", "animal" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                column: "Tags",
                value: new[] { "cat", "pet", "animal" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 13,
                column: "Tags",
                value: new[] { "cat", "pet", "animal", "chair", "furniture" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 14,
                column: "Tags",
                value: new[] { "cat", "pet", "animal", "yacht", "see" });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                column: "Tags",
                value: new[] { "cat", "pet", "animal", "nature" });

        }
    }
}
