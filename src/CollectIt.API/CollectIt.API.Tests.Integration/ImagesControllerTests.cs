using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.Database.Entities.Account;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

public class ImagesControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    
    private readonly CollectItWebApplicationFactory _factory;

    public ImagesControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetImageById_WithValidId_ShouldReturnRequiredImage()
    {
        var image = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadImageDTO>(_factory, $"api/images/1");
        Assert.Equal(image.Name, "Мониторы с аниме");
    }

}