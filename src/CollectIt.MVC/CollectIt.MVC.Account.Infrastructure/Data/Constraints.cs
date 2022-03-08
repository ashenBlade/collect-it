namespace CollectIt.MVC.Account.Infrastructure.Data;

internal static class Constraints
{
    internal static class Subscriptions
    {
        public const string PriceNotNegative = "price_not_negative";
        public const string DaysDurationPositive = "days_duration_positive";
        public const string SubscriptionsPrimaryKey = "subscriptions_primary_key";
        public const string MaxResourcesCountNullOrPositive = "max_resources_null_or_positive";
    }

    internal static class Roles
    {
        public const string RolesPrimaryKey = "roles_primary_key";
    }

    internal static class Users
    {
        public const string UsersPrimaryKey = "users_primary_key";
    }

    internal static class UsersRoles
    {
        public const string NoRolesDuplicatesForUser = "no_roles_duplicates_for_user";
        public const string UserIdForeignKey = "user_id_foreign_key";
        public const string RoleIdForeignKey = "role_id_foreign_key";
    }
    
    internal static class UsersSubscriptions
    {
        public const string Max1SubscriptionPerUserAtTime = "max_1_subscription_per_user_at_time";
        public const string UsersSubscriptionsPrimaryKey = "users_subscriptions_primary_key";
        public const string UsedResourcesCountNotNegative = "used_resources_count_not_negative";
        public const string UsersSubscriptionsUserUserIdForeignKey = "users_subscriptions_users_user_id_foreign_key";

        public const string UsersSubscriptionsSubscriptionsSubscriptionIdForeignKey =
            "users_subscriptions_subscriptions_subscription_id_foreign_key";
    }
    
    
}