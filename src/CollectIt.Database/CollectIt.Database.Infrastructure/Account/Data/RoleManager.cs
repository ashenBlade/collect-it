using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollectIt.Database.Infrastructure.Account.Data;

public class RoleManager : RoleManager<Role>
{
    public RoleManager(IRoleStore<Role> store, 
                       IEnumerable<IRoleValidator<Role>> roleValidators,
                       ILookupNormalizer keyNormalizer,
                       IdentityErrorDescriber errors,
                       ILogger<RoleManager> logger) 
        : base(store, roleValidators, keyNormalizer, errors, logger) 
    { }

    public Task<List<Role>> GetAllRoles()
    {
        return Roles.ToListAsync();
    }

    public Task<Role?> FindByIdAsync(int roleId)
    {
        return Roles.SingleOrDefaultAsync(r => r.Id == roleId);
    }
}