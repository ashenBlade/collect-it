using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddAdditionalRestrictionsForSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.AddColumn<int>(
                name: "RestrictionId",
                table: "Subscriptions",
                type: "integer",
                nullable: true);

            builder.CreateTable(
                name: "Restriction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestrictionType = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: true),
                    DaysAfter = table.Column<int>(type: "integer", nullable: true),
                    DaysTo = table.Column<int>(type: "integer", nullable: true),
                    SizeBytes = table.Column<int>(type: "integer", nullable: true),
                    Tags = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restriction_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            builder.CreateIndex(
                name: "IX_Subscriptions_RestrictionId",
                table: "Subscriptions",
                column: "RestrictionId",
                unique: true);

            builder.CreateIndex(
                name: "IX_Restriction_AuthorId",
                table: "Restriction",
                column: "AuthorId");

            builder.AddForeignKey(
                name: "FK_Subscriptions_Restriction_RestrictionId",
                table: "Subscriptions",
                column: "RestrictionId",
                principalTable: "Restriction",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder builder)
        {
            builder.DropForeignKey(
                name: "FK_Subscriptions_Restriction_RestrictionId",
                table: "Subscriptions");

            builder.DropTable(
                name: "Restriction");

            builder.DropIndex(
                name: "IX_Subscriptions_RestrictionId",
                table: "Subscriptions");

            builder.DropColumn(
                name: "RestrictionId",
                table: "Subscriptions");
        }
    }
}
