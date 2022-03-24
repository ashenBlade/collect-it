using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;

namespace CollectIt.Database.Infrastructure.Account;

public class PostgresqlSubscriptionService : ISubscriptionService
{
    private readonly PostgresqlCollectItDbContext _context;

    public PostgresqlSubscriptionService(PostgresqlCollectItDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserSubscription> SubscribeUserAsync(int userId, int subscriptionId, DateTime startDate)
    {
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId);
        if (subscription is null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        var userSubscription = new UserSubscription()
                               {
                                   UserId = userId,
                                   SubscriptionId = subscriptionId,
                                   During =
                                       new DateInterval(LocalDate.FromDateTime(startDate),
                                                        LocalDate.FromDateTime( startDate.AddMonths(subscription.MonthDuration) )),
                                   LeftResourcesCount = subscription.MaxResourcesCount
                               };
        try
        {
            var result = await _context.UsersSubscriptions.AddAsync(userSubscription);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (DbUpdateException updateException)
        {
            if (updateException.InnerException is not PostgresException postgresException)
            {
                throw;
            }
            
            switch (postgresException.ConstraintName)
            {
                case "MAX_1_SUBSCRIPTION_OF_SAME_TYPE_PER_USER_AT_TIME":
                    throw new UserAlreadySubscribedException(userId, subscriptionId);
                case "FK_UsersSubscriptions_AspNetUsers_UserId":
                    throw new UserNotFoundException(userId);
                case "FK_UsersSubscriptions_Subscriptions_SubscriptionId":
                    throw new SubscriptionNotFoundException(subscriptionId);
                default:
                    throw;
            }
        }
    }
}