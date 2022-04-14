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
    
    private async Task<(HttpClient, string)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
                                                              helper: _testOutputHelper, 
                                                              username: username, 
                                                              password: password);
        return ( client, bearer );
    }

    [Fact]
    public async Task GetRolesList_ShouldReturnRolesList()
    {
        var (client, bearer) = await Initialize();
        var roles = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO[]>(client, 
                                                                                         "api/v1/roles",
                                                                                         bearer);
        Assert.NotEmpty(roles);
        Assert.Contains(roles, r => r.Name == "Admin");
    }

    [Fact]
    public async Task GetRoleById_WithValidId_ShouldReturnRequiredRole()
    {
        var (client, bearer) = await Initialize();
        var adminRole = await TestsHelpers.GetResultParsedFromJson<AccountDTO.ReadRoleDTO>(client, $"api/v1/roles/{Role.AdminRoleId}", bearer);
        Assert.Equal(Role.AdminRoleName, adminRole.Name);
        Assert.Equal(Role.AdminRoleId, adminRole.Id);
    }

    [Fact]
    public async Task GetRoleById_WithUnexistingId_ShouldReturnNotFound()
    {
        await TestsHelpers.AssertNotFoundAsync(_factory, "api/v1/roles/9");
    }
}