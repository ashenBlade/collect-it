using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

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
    public async Task Get_UsersList_Return4InitialUsers()
    {
        var users = await GetResultParsedFromJson<AccountDTO.ReadUserDTO[]>("api/v1/users");
        Assert.Equal(4, users.Length);
    }

    [Fact]
    public async Task Get_UserById_ReturnRequiredUser()
    {
        var user = await GetResultParsedFromJson<AccountDTO.ReadUserDTO>("api/v1/users/1");
        _testOutputHelper.WriteLine(user.UserName);
    }

    private async Task<T> GetResultParsedFromJson<T>(string address, HttpMethod? method = null)
    {
        using var client = _factory.CreateClient();
        using var message = new HttpRequestMessage(method ?? HttpMethod.Get, address);
        var result = await client.SendAsync(message);
        var json = await result.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(json);
        var serializer = JsonSerializer.Create();
        return serializer.Deserialize<T>(new JsonTextReader(new StreamReader(await result.Content.ReadAsStreamAsync())));
    }
}