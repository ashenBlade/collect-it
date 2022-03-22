using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations.Resources
{
    public partial class FixedResourceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Resource_TargetResourceId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Resource_ResourceId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Musics_Resource_ResourceId",
                table: "Musics");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_User_ResourceOwnerId",
                table: "Resource");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Resource_ResourceId",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource",
                table: "Resource");

            migrationBuilder.RenameTable(
                name: "Resource",
                newName: "Resources");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_ResourceOwnerId",
                table: "Resources",
                newName: "IX_Resources_ResourceOwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resources",
                table: "Resources",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Resources_TargetResourceId",
                table: "Comments",
                column: "TargetResourceId",
                principalTable: "Resources",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Resources_ResourceId",
                table: "Images",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Musics_Resources_ResourceId",
                table: "Musics",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_User_ResourceOwnerId",
                table: "Resources",
                column: "ResourceOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Resources_ResourceId",
                table: "Videos",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Resources_TargetResourceId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Resources_ResourceId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Musics_Resources_ResourceId",
                table: "Musics");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_User_ResourceOwnerId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Resources_ResourceId",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resources",
                table: "Resources");

            migrationBuilder.RenameTable(
                name: "Resources",
                newName: "Resource");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_ResourceOwnerId",
                table: "Resource",
                newName: "IX_Resource_ResourceOwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource",
                table: "Resource",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Resource_TargetResourceId",
                table: "Comments",
                column: "TargetResourceId",
                principalTable: "Resource",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Resource_ResourceId",
                table: "Images",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Musics_Resource_ResourceId",
                table: "Musics",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_User_ResourceOwnerId",
                table: "Resource",
                column: "ResourceOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Resource_ResourceId",
                table: "Videos",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "ResourceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
