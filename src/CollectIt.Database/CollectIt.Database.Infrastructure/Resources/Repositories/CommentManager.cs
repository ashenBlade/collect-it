using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.Database.Infrastructure.Resources.Repositories;

public class CommentManager : ICommentManager
{
    private readonly PostgresqlCollectItDbContext context;

    public CommentManager(PostgresqlCollectItDbContext context)
    {
        this.context = context;
    }
    
    public async Task<int> AddAsync(Comment item)
    {
        await context.Comments.AddAsync(item);
        await context.SaveChangesAsync();
        return item.CommentId;
    }

    public async Task<Comment> FindByIdAsync(int id)
    {
        // Better SingleOrDefaultAsync
        return await context.Comments.Where(com => com.CommentId == id).SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Comment item)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Comment item)
    {
        context.Comments.Remove(item);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Comment>> GetResourcesComments(int resourceId)
    {
        return await context.Comments
            .Include(com => com.Target)
            .Include(com => com.Owner)
            .Where(com => com.Target.Id == resourceId).ToListAsync();
    }
}