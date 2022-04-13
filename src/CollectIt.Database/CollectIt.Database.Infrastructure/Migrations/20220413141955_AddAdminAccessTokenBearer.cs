using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddAdminAccessTokenBearer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OpenIddictTokens",
                columns: new[] { "Id", "ApplicationId", "AuthorizationId", "ConcurrencyToken", "CreationDate", "ExpirationDate", "Payload", "Properties", "RedemptionDate", "ReferenceId", "Status", "Subject", "Type" },
                values: new object[] { 1, null, null, "05fa1fe4-a237-4abc-a242-fa56c18c08ee", new DateTime(2022, 4, 13, 14, 13, 30, 0, DateTimeKind.Utc), new DateTime(2025, 4, 12, 14, 13, 30, 0, DateTimeKind.Utc), null, null, null, null, "valid", "1", "access_token" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OpenIddictTokens",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
