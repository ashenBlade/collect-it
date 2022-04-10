using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddIndexForFullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "NameSearchVector",
                table: "Resources",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "russian")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name" });
            
            migrationBuilder.CreateIndex(
                name: "IX_Resources_NameSearchVector",
                table: "Resources",
                column: "NameSearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2022, 3, 27, 10, 56, 59, 207, DateTimeKind.Utc));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resources_NameSearchVector",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "NameSearchVector",
                table: "Resources");

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                column: "UploadDate",
                value: new DateTime(2022, 3, 26, 12, 34, 6, 430, DateTimeKind.Utc).AddTicks(9100));
        }
    }
}
