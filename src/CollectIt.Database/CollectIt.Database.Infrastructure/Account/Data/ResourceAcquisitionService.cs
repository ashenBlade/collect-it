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
        if (image is null)
        {
            throw new ResourceNotFoundException(image.Id, $"Image with Id = {imageId} not found");
        }

        return await AcquireResourceAsync(userId, image);
    }

    public Task<AcquiredUserResource> AcquireMusicAsync(int userId, int musicId)
    {
        throw new NotImplementedException("No music manager implemented yet");
    }

    public Task<AcquiredUserResource> AcquireVideoAsync(int userId, int videoId)
    {
        throw new NotImplementedException("No video manager implemented yet");
    }

    private async Task<AcquiredUserResource> AcquireResourceAsync(int userId, Resource resource)
    {
        var subscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(userId);
        var affordable =
            subscriptions.FirstOrDefault(s => s.Subscription.Restriction is null
                                           || s.Subscription.Restriction.IsSatisfiedBy(resource));
        if (affordable is null)
        {
            throw new NoSuitableSubscriptionFoundException($"No suitable subscription found to acquire image (Id = {resource.Id}) for user (Id = {userId})");
        }
        var acquiredUserResource = new AcquiredUserResource()
                                   {
                                       UserId = userId, ResourceId = resource.Id, AcquiredDate = DateTime.Today,
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
                                                                     new UserAlreadyAcquiredResourceException(resource.Id, userId),
                                                                 _ => postgresException
                                                             },
                      _ => exception
                  };
        }
    }
}