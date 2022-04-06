using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations
{
    public partial class ChangeActiveUsersSubscriptionsView : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.Sql(@"    
    DROP VIEW ""ActiveUsersSubscriptions"";
    CREATE OR REPLACE VIEW public.""ActiveUsersSubscriptions"" AS (
        SELECT us.""Id"",
            us.""UserId"",
            us.""SubscriptionId"",
            us.""LeftResourcesCount"",
            us.""During""
        FROM ""UsersSubscriptions"" AS us
        WHERE us.""During"" @> current_date);");
        }

        protected override void Down(MigrationBuilder builder)
        {
            builder.Sql(@"
    DROP VIEW ""ActiveUsersSubscriptions"";
    CREATE VIEW public.""ActiveUsersSubscriptions"" AS (
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
    }
}
