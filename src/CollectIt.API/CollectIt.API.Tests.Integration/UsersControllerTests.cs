using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CollectIt.API.Tests.Integration;

public class UsersControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public UsersControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Get_UsersList_ReturnUsersArray()
    {
        var r = await GetResultParsedFromJson<AccountDTO.ReadUserDTO[]>("api/v1/users");
        foreach (var user in r)
        {
            _testOutputHelper.WriteLine($"User (Id: {user.Id}; Name: {user.UserName}; Email: {user.Email})");
            _testOutputHelper.WriteLine("");
        }
    }

    private async Task<T> GetResultParsedFromJson<T>(string address, HttpMethod? method = null)
    {
        using var client = _factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        var result = await client.SendAsync(message);
        var json = await result.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(json);
        return JsonSerializer.Deserialize<T>(json) 
            ?? throw new HttpRequestException($"Could not deserialize response to type: {typeof(T)}\nActual result: \"{json}\"");
    }
}