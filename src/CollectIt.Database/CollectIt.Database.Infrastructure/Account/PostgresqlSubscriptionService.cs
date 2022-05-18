using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;

namespace CollectIt.Database.Infrastructure.Account;

public class PostgresqlSubscriptionService : ISubscriptionService
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly ILogger<PostgresqlSubscriptionService> _logger;

    public PostgresqlSubscriptionService(PostgresqlCollectItDbContext context,
                                         ILogger<PostgresqlSubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserSubscription> SubscribeUserAsync(int userId, int subscriptionId, DateTime startDate)
    {
        var subscription = await _context.Subscriptions
                                         .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        if (subscription is null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        if (!subscription.Active)
        {
            throw new SubscriptionIsNotActiveException(subscriptionId);
        }

        DateTime endDate;
        try
        {
            endDate = startDate.AddMonths(subscription.MonthDuration);
        }
        catch (ArgumentOutOfRangeException outOfRange)
        {
            _logger.LogError(outOfRange,
                             "Adding {MonthsCount} months to start date {StartDate} for subscription {SubscriptionId} caused {ExceptionName}\nSetting end date to MaxDate",
                             subscription.MonthDuration,
                             startDate,
                             subscriptionId,
                             nameof(ArgumentOutOfRangeException));
            endDate = DateTime.MaxValue;
        }

        var userSubscription = new UserSubscription
                               {
                                   UserId = userId,
                                   SubscriptionId = subscriptionId,
                                   During =
                                       new DateInterval(LocalDate.FromDateTime(startDate),
                                                        LocalDate.FromDateTime(endDate)),
                                   LeftResourcesCount = subscription.MaxResourcesCount
                               };
        try
        {
            var result = await _context.UsersSubscriptions.AddAsync(userSubscription);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User (UserId = {UserId}) subscribed to subscription (SubscriptionId = {SubscriptionId}). Created subscription id = {CreatedUserSubscriptionId}",
                                   userId, subscriptionId, result.Entity.Id);
            return result.Entity;
        }
        catch (DbUpdateException updateException)
        {
            if (updateException.InnerException is not PostgresException postgresException)
                throw;

            throw postgresException.ConstraintName switch
                  {
                      "MAX_1_SUBSCRIPTION_OF_SAME_TYPE_PER_USER_AT_TIME" =>
                          new UserAlreadySubscribedException(userId, subscriptionId),
                      "FK_UsersSubscriptions_AspNetUsers_UserId" =>
                          new UserNotFoundException(userId),
                      "FK_UsersSubscriptions_Subscriptions_SubscriptionId" =>
                          new SubscriptionNotFoundException(subscriptionId),
                      _ => postgresException
                  };
        }
    }
}