using System.Data.Common;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class ResourceAcquisitionService : IResourceAcquisitionService
{
    private readonly UserManager _userManager;
    private readonly PostgresqlCollectItDbContext _context;
    private readonly IImageManager _imageManager;

    public ResourceAcquisitionService(UserManager userManager, 
                          PostgresqlCollectItDbContext context,
                          IImageManager imageManager)
    {
        _userManager = userManager;
        _context = context;
        _imageManager = imageManager;
    }
    
    public Task<bool> IsResourceAcquiredByUserAsync(int userId, int resourceId)
    {
        var resource = new Resource() {Id = resourceId};
        return _context.Users
                       .AnyAsync(u => u.AcquiredResources.Contains(resource));
    }

    public async Task<AcquiredUserResource> AcquireImageAsync(int userId, int imageId)
    {
        var image = await _imageManager.FindByIdAsync(imageId);
        var subscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(userId);
        var affordable =
            subscriptions.FirstOrDefault(s => s.Subscription.Restriction is null
                                           || s.Subscription.Restriction.IsSatisfiedBy(image));
        if (affordable is null)
        {
            throw new NoSuitableSubscriptionFoundException($"No suitable subscription found to acquire image (Id = {imageId}) for user (Id = {userId})");
        }
        var acquiredUserResource = new AcquiredUserResource()
                                   {
                                       UserId = userId, ResourceId = image.Id, AcquiredDate = DateTime.Today,
                                   };
        try
        {
            var result = await _context.AcquiredUserResources.AddAsync(acquiredUserResource);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (DbUpdateException exception)
        {
            throw exception.InnerException switch
                  {
                      PostgresException postgresException => postgresException.ConstraintName switch
                                                          {
                                                              "AK_AcquiredUserResources_UserId_ResourceId" =>
                                                                  new UserAlreadyAcquiredResourceException(image.Id, userId),
                                                              _ => postgresException
                                                          },
                      _                 => exception
                  };
        }
    }

    public Task<AcquiredUserResource> AcquireMusicAsync(int userId, int musicId)
    {
        throw new NotImplementedException();
    }

    public Task<AcquiredUserResource> AcquireVideoAsync(int userId, int videoId)
    {
        throw new NotImplementedException();
    }
}