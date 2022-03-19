using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectIt.MVC.View.Migrations.Accounts
{
    public partial class AddActiveUsersSubscriptionsView : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.Sql(@"
    CREATE VIEW ""ActiveUsersSubscriptions"" AS (
        SELECT us.""UserId"", us.""SubscriptionId"", us.""During"", us.""LeftResourcesCount"", s.""MaxResourcesCount"" 
        FROM ""UsersSubscriptions"" AS us 
            JOIN ""Subscriptions"" AS s ON us.""SubscriptionId"" = s.""Id""
        WHERE 
              us.""LeftResourcesCount"" > 0 AND
              us.""During"" @> current_date 
        );
");
        }

        protected override void Down(MigrationBuilder builder)
        {
            builder.Sql(@"DROP VIEW ""ActiveUsersSubscriptions"";");
        }
    }
}
