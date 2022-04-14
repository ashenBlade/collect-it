using System.Data.Common;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class SubscriptionManager : ISubscriptionManager
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly ILogger<SubscriptionManager> _logger;

    public SubscriptionManager(PostgresqlCollectItDbContext context,
                               ILogger<SubscriptionManager> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Subscription> CreateSubscriptionAsync(string name,
                                                            string description,
                                                            int monthDuration,
                                                            ResourceType appliedResourceType,
                                                            int maxResourcesCount,
                                                            int? restrictionId,
                                                            bool active = false)
    {
        var subscription = new Subscription()
                           {
                               Name = name,
                               Description = description,
                               MonthDuration = monthDuration,
                               MaxResourcesCount = maxResourcesCount,
                               RestrictionId = restrictionId,
                               Active = true
                           };
        var result = await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New subscription added: Id = {Id}, Name = {Name}", subscription.Id, subscription.Name);
        return result.Entity;
    }

    public Task<List<Subscription>> GetSubscriptionsAsync(int pageNumber, int pageSize)
    {
        return _context.Subscriptions
                       .OrderBy(s => s.Id)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    public Task<List<Subscription>> GetSubscriptionsAsync(ResourceType resourceType, int pageNumber, int pageSize)
    {
        return _context.Subscriptions
                       .Where(s => s.AppliedResourceType == resourceType)
                       .Skip(( pageNumber - 1 ) * pageSize)
                       .Take(pageSize)
                       .ToListAsync();
    }

    public Task<List<Subscription>> GetActiveSubscriptionsAsync(int pageNumber, int pageSize)
    {
        return ActiveSubscriptions
              .OrderBy(s => s.Id)
              .Skip(( pageNumber - 1 ) * pageSize)
              .Take(pageSize)
              .ToListAsync();
    }

    private IQueryable<Subscription> ActiveSubscriptions
    {
        get
        {
            return _context.Subscriptions
                           .Where(s => s.Active);
        }
    }

    public Task<List<Subscription>> GetActiveSubscriptionsAsync()
    {
        return _context.Subscriptions
                       .Where(s => s.Active)
                       .ToListAsync();
    }

    public async Task DeleteSubscriptionAsync(int id)
    {
        var subscription = new Subscription() {Id = id};
        _context.Subscriptions.Attach(subscription);
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Subscription with Id = {SubscriptionId} was deleted", id);
    }
    
    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType)
    {
        return _context.Subscriptions
                       .Include(s => s.Restriction)
                       .Where(s => s.AppliedResourceType == resourceType)
                       .ToListAsync();
    }

    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType, int pageNumber, int pageSize)
    {
        return ActiveSubscriptions
              .Include(s => s.Restriction)
              .Where(s => s.AppliedResourceType == resourceType)
              .OrderBy(s => s.Id)
              .Skip(( pageNumber - 1 ) * pageSize)
              .Take(pageSize)
              .ToListAsync();
    }

    public Task<IdentityResult> ActivateSubscriptionAsync(int subscriptionId)
    {
        return SetActiveSubscriptionState(subscriptionId, true);
    }

    public Task<IdentityResult> DeactivateSubscriptionAsync(int subscriptionId)
    {
        return SetActiveSubscriptionState(subscriptionId, false);
    }

    public async Task<IdentityResult> ChangeSubscriptionNameAsync(int subscriptionId, string newName)
    {
        await using var connection = _context.Database.GetDbConnection();
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "UPDATE \"Subscriptions\" SET \"Name\" = @name WHERE \"Id\" = @id;";
            command.Parameters.Add(CreateParameter(command, "@name", newName));
            command.Parameters.Add(CreateParameter(command, "@id", subscriptionId));
            await connection.OpenAsync();
            var affected = await command.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return IdentityResult.Failed(new IdentityError()
                                             {
                                                 Code = "", Description = "No subscription with provided id found"
                                             });
            }

            return IdentityResult.Success;
        }
        catch (DbUpdateException updateException)
        {
            _logger.LogError(updateException, "Error while updating name for subscription");
            return IdentityResult.Failed(new IdentityError()
                                         {
                                             Code = "", Description = "Error while updating on database"
                                         });
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static DbParameter CreateParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        return parameter;
    }

    public async Task<IdentityResult> ChangeSubscriptionDescriptionAsync(int subscriptionId, string newDescription)
    {
        await using var connection = _context.Database.GetDbConnection();
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "UPDATE \"Subscriptions\" SET \"Description\" = @description WHERE \"Id\" = @id;";
            command.Parameters.Add(CreateParameter(command, "@description", newDescription));
            command.Parameters.Add(CreateParameter(command, "@id", subscriptionId));
            await connection.OpenAsync();
            var affected = await command.ExecuteNonQueryAsync();
            if (affected == 0)
            {
                return IdentityResult.Failed(new IdentityError()
                                             {
                                                 Code = "", Description = "No subscription with provided id found"
                                             });
            }

            return IdentityResult.Success;
        }
        catch (DbUpdateException updateException)
        {
            _logger.LogError(updateException, "Error while updating name for subscription");
            return IdentityResult.Failed(new IdentityError()
                                         {
                                             Code = "", Description = "Error while updating on database"
                                         });
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private async Task<IdentityResult> SetActiveSubscriptionState(int subscriptionId, bool isActive)
    {
        try
        {
            var subscription = await _context.Subscriptions
                                             .SingleOrDefaultAsync(s => s.Id == subscriptionId);
            if (subscription is null)
            {
                throw new SubscriptionNotFoundException(subscriptionId);
            }

            if (subscription.Active == isActive)
                return IdentityResult.Success;

            subscription.Active = isActive;
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }
        catch (DbUpdateException updateException)
        {
            _logger.LogError(updateException, "Error while activating/deactivating subscription");
            return IdentityResult.Failed(new IdentityError()
                                         {
                                             Code = "",
                                             Description = isActive
                                                               ? "Could not activate subscription"
                                                               : "Could not deactivate subscription"
                                         });
        }
        catch (SubscriptionNotFoundException subscriptionNotFoundException)
        {
            return IdentityResult.Failed(new IdentityError()
                                         {
                                             Code = "SubscriptionNotFound",
                                             Description = $"Subscription with Id = {subscriptionId} not found"
                                         });
        }
    }

    public Task<Subscription?> FindSubscriptionByIdAsync(int id)
    {
        return _context.Subscriptions
                       .Include(s => s.Restriction)
                       .Where(s => s.Id == id)
                       .SingleOrDefaultAsync();
    }
}