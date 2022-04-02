using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class Add_Tags_FileNames_Extensions_And_Rename_Path : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Resources",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "Tags",
                table: "Resources",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/abstract-img.jpg", "jpg", "abstract-img.jpg", new[] { "anime", "fallout" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/bird-img.jpg", "jpg", "bird-img.jpg", new[] { "bird", "nature" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/car-img.jpg", "jpg", "car-img.jpg", new[] { "car" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img.jpg", "jpg", "cat-img.jpg", new[] { "cat", "animal", "pet" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/house-img.jpg", "jpg", "house-img.jpg", new[] { "house" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/nature-img.jpg", "jpg", "nature-img.jpg", new[] { "nature" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/school-img.jpg", "jpg", "school-img.jpg", new[] { "school", "kids" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-2.jpg", "jpg", "cat-img-2.jpg", new[] { "cat", "pet", "animal" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-3.jpg", "jpg", "cat-img-3.jpg", new[] { "cat", "pet", "animal", "sunglasses" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-4.jpg", "jpg", "cat-img-4.jpg", new[] { "cat", "pet", "animal" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-5.jpg", "jpg", "cat-img-5.jpg", new[] { "cat", "pet", "animal" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-6.jpg", "jpg", "cat-img-6.jpg", new[] { "cat", "pet", "animal" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-7.jpg", "jpg", "cat-img-7.jpg", new[] { "cat", "pet", "animal", "chair", "furniture" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-8.jpg", "jpg", "cat-img-8.jpg", new[] { "cat", "pet", "animal", "yacht", "see" } });

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Address", "Extension", "FileName", "Tags" },
                values: new object[] { "/imagesFromDb/cat-img-9.jpg", "jpg", "cat-img-9.jpg", new[] { "cat", "pet", "animal", "nature" } });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Resources",
                newName: "Path");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                column: "Path",
                value: "/imagesFromDb/abstract-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2,
                column: "Path",
                value: "/imagesFromDb/bird-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3,
                column: "Path",
                value: "/imagesFromDb/car-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4,
                column: "Path",
                value: "/imagesFromDb/cat-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5,
                column: "Path",
                value: "/imagesFromDb/house-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 6,
                column: "Path",
                value: "/imagesFromDb/nature-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 7,
                column: "Path",
                value: "/imagesFromDb/school-img.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 8,
                column: "Path",
                value: "/imagesFromDb/cat-img-2.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 9,
                column: "Path",
                value: "/imagesFromDb/cat-img-3.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 10,
                column: "Path",
                value: "/imagesFromDb/cat-img-4.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 11,
                column: "Path",
                value: "/imagesFromDb/cat-img-5.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 12,
                column: "Path",
                value: "/imagesFromDb/cat-img-6.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 13,
                column: "Path",
                value: "/imagesFromDb/cat-img-7.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 14,
                column: "Path",
                value: "/imagesFromDb/cat-img-8.jpg");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 15,
                column: "Path",
                value: "/imagesFromDb/cat-img-9.jpg");
        }
    }
}
