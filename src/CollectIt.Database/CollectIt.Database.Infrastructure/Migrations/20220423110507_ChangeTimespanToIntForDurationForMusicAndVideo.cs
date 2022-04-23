

#nullable disable

using System;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Migrations;
namespace CollectIt.MVC.View.Migrations
{
    public partial class ChangeTimespanToIntForDurationForMusicAndVideo : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.DropColumn("Duration", "Videos");
            builder.AddColumn<int>("Duration", "Videos", "integer", nullable: false);
            builder.DropColumn("Duration", "Musics");
            builder.AddColumn<int>("Duration", "Musics", "integer", nullable: false);
        }

        protected override void Down(MigrationBuilder builder)
        {
            builder.DropColumn("Duration", "Videos");
            builder.AddColumn<TimeSpan>("Duration", "Videos", "interval", nullable: false);
            builder.DropColumn("Duration", "Musics");
            builder.AddColumn<TimeSpan>("Duration", "Musics", "interval", nullable: false);
        }
    }
}
