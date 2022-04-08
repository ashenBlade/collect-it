using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface ICommentManager : IRepository<Comment, int>
{
    Task<IEnumerable<Comment>> GetResourcesComments(int resourceId);
}