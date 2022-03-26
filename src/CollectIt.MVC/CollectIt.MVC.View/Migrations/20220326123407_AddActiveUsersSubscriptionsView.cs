using System;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class AddActiveUsersSubscriptionsView : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.Sql(sql: @"
    CREATE VIEW ""ActiveUsersSubscriptions"" AS (
        SELECT us.""UserId"", 
               us.""SubscriptionId"", 
               us.""LeftResourcesCount"", 
               us.""During"", 
               s.""MaxResourcesCount"" 
        FROM ""UsersSubscriptions"" AS us
        JOIN ""Subscriptions"" AS s 
            ON us.""SubscriptionId"" = s.""Id""
        WHERE us.""During"" @> current_date
    );
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW \"ActiveUsersSubscriptions\";");
        }
    }
}
