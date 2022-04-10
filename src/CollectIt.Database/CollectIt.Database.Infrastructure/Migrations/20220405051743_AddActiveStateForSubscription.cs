using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddActiveStateForSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Subscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Active",
                table: "Subscriptions",
                column: "Active");
            
            migrationBuilder.UpdateData(table: "Subscriptions",
                                        keyColumn: "Id",
                                        keyValue: 1,
                                        column: "Active",
                                        value: true);

            migrationBuilder.UpdateData(table: "Subscriptions",
                                        keyColumn: "Id",
                                        keyValue: 2,
                                        column: "Active",
                                        value: true);

            migrationBuilder.UpdateData(table: "Subscriptions",
                                        keyColumn: "Id",
                                        keyValue: 3,
                                        column: "Active",
                                        value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Active",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Subscriptions");
        }
    }
}
