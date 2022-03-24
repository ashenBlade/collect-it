using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollectIt.Database.Infrastructure.Account.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager _userManager;

    public UserRepository(UserManager userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<int> AddAsync(User user)
    {
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            return user.Id;
        }

        throw new UserException(user.Id, result.Errors
                                               .Select(i => i.Description)
                                               .Aggregate((s, n) => $"{s} {n}"));
    }

    public Task RemoveAsync(User user)
    {
        return _userManager.DeleteAsync(user);
    }

    public Task<User?> FindByIdAsync(int id)
    {
        return _userManager.FindByIdAsync(id.ToString())!;
    }

    public Task UpdateAsync(User user)
    {
        return _userManager.UpdateAsync(user);
    }
}