using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class FixedInheritanceHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_ResourceId",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Musics",
                table: "Musics");

            migrationBuilder.DropIndex(
                name: "IX_Musics_ResourceId",
                table: "Musics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ResourceId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "Videos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MusicId",
                table: "Musics",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Images",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Videos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Musics",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Images",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Musics",
                table: "Musics",
                column: "ResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "ResourceId");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoleId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "3e0213e9-8d80-48df-b9df-18fc7debd84e", "asdf@mail.ru", false, false, null, "ASDF@MAIL.RU", null, "AQAAAAEAACcQAAAAEAO/K1C4Jn77AXrULgaNn6rkHlrkXbk9jOqHqe+HK+CvDgmBEEFahFadKE8H7x4Olw==", null, false, null, "MSCN3JBQERUJBPLR4XIXZH3TQGICF6O3", false, "asdf@mail.ru" });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "ResourceId", "ResourceOwnerId", "ResourcePath", "UploadDate" },
                values: new object[] { 1, 1, "/imagesFromDb/avaSig.jpg", new DateTime(2022, 3, 26, 10, 6, 13, 834, DateTimeKind.Utc).AddTicks(1996) });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "ResourceId", "Id" },
                values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Musics",
                table: "Musics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "ResourceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Videos",
                newName: "VideoId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Musics",
                newName: "MusicId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Images",
                newName: "ImageId");

            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "Videos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "MusicId",
                table: "Musics",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "Images",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Musics",
                table: "Musics",
                column: "MusicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ResourceId",
                table: "Videos",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Musics_ResourceId",
                table: "Musics",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ResourceId",
                table: "Images",
                column: "ResourceId");
        }
    }
}
