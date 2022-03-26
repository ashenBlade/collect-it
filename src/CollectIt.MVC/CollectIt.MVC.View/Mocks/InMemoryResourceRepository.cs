using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;

namespace CollectIt.MVC.View.Mocks;

public class InMemoryResourceRepository : IResourceRepository
{
    public Task<int> AddAsync(Resource item)
    {
        throw new NotImplementedException();
    }

    public Task<Resource?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Resource item)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Resource item)
    {
        throw new NotImplementedException();
    }
}