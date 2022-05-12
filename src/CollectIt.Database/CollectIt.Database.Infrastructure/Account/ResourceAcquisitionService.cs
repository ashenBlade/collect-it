using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CollectIt.Database.Infrastructure.Account;

public class ResourceAcquisitionService : IResourceAcquisitionService
{
    private readonly PostgresqlCollectItDbContext _context;
    private readonly IImageManager _imageManager;
    private readonly IMusicManager _musicManager;
    private readonly UserManager _userManager;
    private readonly IVideoManager _videoManager;

    public ResourceAcquisitionService(UserManager userManager,
                                      PostgresqlCollectItDbContext context,
                                      IImageManager imageManager,
                                      IMusicManager musicManager,
                                      IVideoManager videoManager)
    {
        _userManager = userManager;
        _context = context;
        _imageManager = imageManager;
        _musicManager = musicManager;
        _videoManager = videoManager;
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

    public async Task<AcquiredUserResource> AcquireMusicAsync(int userId, int musicId)
    {
        var music = await _musicManager.FindByIdAsync(musicId);
        if (music is null)
        {
            throw new ResourceNotFoundException(musicId, $"Music with Id = {musicId} not found");
        }

        return await AcquireResourceAsync(userId, music);
    }

    public async Task<AcquiredUserResource> AcquireVideoAsync(int userId, int videoId)
    {
        var music = await _videoManager.FindByIdAsync(videoId);
        if (music is null)
        {
            throw new ResourceNotFoundException(videoId, $"Music with Id = {videoId} not found");
        }

        return await AcquireResourceAsync(userId, music);
    }

    private async Task<AcquiredUserResource> AcquireResourceAsync(int userId, Resource resource)
    {
        var subscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(userId);
        var affordable =
            subscriptions.FirstOrDefault(s => s.Subscription.Restriction is null
                                           || s.Subscription.Restriction.IsSatisfiedBy(resource));
        if (affordable is null)
        {
            throw new
                NoSuitableSubscriptionFoundException($"No suitable subscription found to acquire resource (Id = {resource.Id}) for user (Id = {userId})");
        }

        var acquiredUserResource = new AcquiredUserResource()
                                   {
                                       UserId = userId, ResourceId = resource.Id, AcquiredDate = DateTime.UtcNow,
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
                                                                     new
                                                                         UserAlreadyAcquiredResourceException(resource.Id,
                                                                                                              userId),
                                                                 _ => postgresException
                                                             },
                      _ => exception
                  };
        }
    }
}