using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class ImagesControllerTests : IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public ImagesControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
    }
    
    private async Task<(HttpClient, string)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
            helper: _outputHelper, 
            username: username, 
            password: password);
        return ( client, bearer );
    }
    
    [Fact]
    public async Task GetImageById_WithValidId_ShouldReturnRequiredImage()
    {
        var (client, bearer) = await Initialize();
        var image = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadImageDTO>(client
            , $"api/images/1"
            , bearer);
        Assert.Equal(image.Name, "Мониторы с аниме");
    }
    
    [Fact]
    public async Task GetImageById_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client, "api/images/1488", HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetImages_WithValidQueryParameters_ShouldReturnListOfImages()
    {
        var (client, bearer) = await Initialize();
        var images = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadImageDTO[]>(client,
            "api/images?page_size=10&page_number=1",
            bearer);
        Assert.NotEmpty(images);
    }
    
    
}