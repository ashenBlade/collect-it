using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddTagsSearchVectorToResource : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.Sql(@"
    CREATE OR REPLACE FUNCTION my_array_to_string(arr ANYARRAY, sep TEXT)
        RETURNS text
        AS $$
        SELECT array_to_string(arr, sep);
    $$
    LANGUAGE SQL
    IMMUTABLE; 
");
            builder.Sql(@"ALTER TABLE ""Resources""
    ADD COLUMN ""TagsSearchVector""
        tsvector
        GENERATED ALWAYS AS (
            to_tsvector('russian', my_array_to_string(""Tags"", ' ') || ' ' || ""Name"")) STORED;");

            builder.Sql(@"CREATE INDEX ""IX_Resources_TagsSearchVector"" 
    ON ""Resources"" 
        USING GIN(""TagsSearchVector"");");
            
            // builder.AddColumn<NpgsqlTsVector>(
            //     name: "TagSearchVector",
            //     table: "Resources",
            //     type: "tsvector",
            //     nullable: false)
            //     .Annotation("Npgsql:TsVectorConfig", "russian")
            //     .Annotation("Npgsql:TsVectorProperties", new[] { "Tags" });
            //
            // builder.CreateIndex(
            //     name: "IX_Resources_TagSearchVector",
            //     table: "Resources",
            //     column: "TagSearchVector")
            //     .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resources_TagsSearchVector",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "TagsSearchVector",
                table: "Resources");

            migrationBuilder.Sql(@"DROP FUNCTION my_array_to_string;");
        }
    }
}
