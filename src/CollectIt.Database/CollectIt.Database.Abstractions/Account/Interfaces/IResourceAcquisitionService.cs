using System.Threading.Tasks;
using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface IResourceAcquisitionService
{
    public Task<bool> IsResourceAcquiredByUserAsync(int userId, int resourceId);
    /// <summary>
    /// Add resource to be available for current user to download
    /// </summary>
    /// <exception cref="CollectIt.Database.Abstractions.Account.Exceptions.NoSuitableSubscriptionFoundException">No subscription found to acquire required image for required user</exception>
    /// <exception cref="CollectIt.Database.Abstractions.Account.Exceptions.UserNotFoundException">User with provided <param name="userId">User Id</param> not found</exception>
    /// <exception cref="CollectIt.Database.Abstractions.Account.Exceptions.ResourceNotFoundException">Image with provided <param name="imageId">Image Id</param> not found </exception>
    /// <exception cref="CollectIt.Database.Abstractions.Account.Exceptions.UserAlreadyAcquiredResourceException">User already had image acquired</exception>
    public Task<AcquiredUserResource> AcquireImageAsync(int userId, int imageId);
    public Task<AcquiredUserResource> AcquireMusicAsync(int userId, int musicId);
    public Task<AcquiredUserResource> AcquireVideoAsync(int userId, int videoId);
    public Task<AcquiredUserResource> AcquireImageWithoutSubscriptionAsync(int userId, int imageId);
    public Task<AcquiredUserResource> AcquireMusicWithoutSubscriptionAsync(int userId, int musicId);
    public Task<AcquiredUserResource> AcquireVideoWithoutSubscriptionAsync(int userId, int videoId);
}