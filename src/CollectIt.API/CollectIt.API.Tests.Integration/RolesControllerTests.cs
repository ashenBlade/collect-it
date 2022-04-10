using System;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class RolesControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public RolesControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetRolesList_ShouldReturnRolesList()
    {
        var roles = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO[]>(_factory, "api/v1/roles");
        Assert.NotEmpty(roles);
        Assert.Contains(roles, r => r.Name == "Admin");
    }

    [Fact]
    public async Task GetRoleById_WithValidId_ShouldReturnRequiredRole()
    {
        var adminRole = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO>(_factory, $"api/v1/roles/{Role.AdminRoleId}");
        Assert.Equal(Role.AdminRoleName, adminRole.Name);
        Assert.Equal(Role.AdminRoleId, adminRole.Id);
    }

    [Fact]
    public async Task GetRoleById_WithUnexistingId_ShouldReturnNotFound()
    {
        await TestsHelpers.AssertNotFoundAsync(_factory, "api/v1/roles/9");
    }
}