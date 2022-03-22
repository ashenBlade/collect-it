using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Resources.Entities;

namespace CollectIt.MVC.Resources.Abstractions;

public interface ICommentRepository : IRepository<Comment, int>
{
    
}