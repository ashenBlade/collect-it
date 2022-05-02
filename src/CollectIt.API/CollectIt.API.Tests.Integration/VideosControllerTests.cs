using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace CollectIt.API.Tests.Integration;

[Collection("Videos")]
public class VideosControllerTests: IClassFixture<CollectItWebApplicationFactory>
{
    private readonly CollectItWebApplicationFactory _factory;
    private readonly ITestOutputHelper _outputHelper;

    public VideosControllerTests(CollectItWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _outputHelper = outputHelper;
    }
    
    private async Task<(HttpClient client, string bearer)> Initialize(string? username = null, string? password = null)
    {
        var client = _factory.CreateClient();
        var bearer = await TestsHelpers.GetBearerForUserAsync(client, 
                                                              helper: _outputHelper, 
                                                              username: username, 
                                                              password: password);
        return ( client, bearer );
    }

    private static Video[] DefaultVideos => PostgresqlCollectItDbContext.DefaultVideos;
    [Fact]
    public async Task GetVideosList_WithValidPageNumberAndSize_ShouldReturnVideosList()
    {
        var (client, bearer) = await Initialize();
        const int PageNumber = 1;
        const int PageSize = 5; 
        var actual = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadVideoDTO[]>(client,
                                                                                             $"api/v1/videos?page_number={PageNumber}&page_size={PageSize}",
                                                                                             bearer,
                                                                                             HttpMethod.Get, _outputHelper);

        var expected = DefaultVideos.OrderBy(v => v.Id).Take(PageSize).ToList();
        Assert.Equal(expected.Count, actual.Length);
        for (int i = 0; i < expected.Count; i++)
        {
            AssertVideosEqual(expected[i], actual[i]);
        }
        client.Dispose();
    }
    
    [Fact]
    public async Task GetVideoById_WithValidId_ShouldReturnVideo()
    {
        var (client, bearer) = await Initialize();
        var video = DefaultVideos.Last();
        var actual = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadVideoDTO>(client,
                                                                                           $"api/v1/videos/{video.Id}",
                                                                                           bearer,
                                                                                           HttpMethod.Get, 
                                                                                           _outputHelper);
        AssertVideosEqual(video, actual);
        client.Dispose();
    }

    [Fact]
    public async Task GetVideoById_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/1000",
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Get, bearer);
        client.Dispose();
    }

    [Fact]
    public async Task ChangeVideoName_WithValidName_ShouldReturnNoContentOnSuccess()
    {
        var (client, bearer) = await Initialize();
        var video = DefaultVideos.First();
        const string NewName = "Some valid new name";
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{video.Id}/name",
                                                 HttpStatusCode.NoContent, HttpMethod.Post, bearer,
                                                 new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                                                                           {
                                                                               new("Name", NewName)
                                                                           }));
        client.Dispose();
    }
    
    [Fact]
    public async Task ChangeVideoName_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        var id = DefaultVideos.Max(v => v.Id) + 1;
        const string NewName = "Some valid new name";
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{id}/name",
                                                 HttpStatusCode.NotFound, HttpMethod.Post, bearer,
                                                 new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                                                                           {
                                                                               new("Name", NewName)
                                                                           }));
        client.Dispose();
    }

    [Fact]
    public async Task ChangeVideoTags_WithValidTags_ShouldReturnNoContentOnSuccess()
    {
        var (client, bearer) = await Initialize();
        var video = DefaultVideos.First();
        var tags = new[] {"tag1", "tag", "another", "cat", "way", "milk"};
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{video.Id}/tags",
                                                 HttpStatusCode.NoContent,
                                                 HttpMethod.Post,
                                                 bearer,
                                                 new FormUrlEncodedContent(tags.Select(t =>
                                                                                           new KeyValuePair<string,
                                                                                               string>("Tags", t))));
        client.Dispose();
    }
    
    [Fact]
    public async Task ChangeVideoTags_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        var id = DefaultVideos.Max(v => v.Id) + 1;
        var tags = new[] {"tag1", "tag", "another", "cat", "way", "milk"};
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{id}/tags",
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Post,
                                                 bearer,
                                                 new FormUrlEncodedContent(tags.Select(t =>
                                                                                           new KeyValuePair<string,
                                                                                               string>("Tags", t))));
        client.Dispose();
    }

    [Fact]
    public async Task CreateNewVideo_WithValidInput_ShouldReturnCreatedVideo()
    {
        var dto = new ResourcesDTO.CreateVideoDTO()
                  {
                      Content = new FormFile(Stream.Null, 0, 0, "SomeName", "FileName"),
                      Duration = 10,
                      Extension = "webp",
                      Name = "Some video name",
                      Tags = new[] {"hello", "best", "dog"},
                      OwnerId = 1,
                      UploadDate = DateTime.UtcNow
                  };
        var (client, bearer) = await Initialize();
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(dto.Name), "Name");
        content.Add(new StringContent(dto.UploadDate.ToString()), "UploadDate");
        content.Add(new StringContent(dto.Extension), "Extension");
        content.Add(new StringContent(dto.Duration.ToString()), "Duration");
        content.Add(new StringContent(dto.OwnerId.ToString()), "OwnerId");
        dto.Tags.ToList().ForEach(t => content.Add(new StringContent(t), "Tags"));
        var byteContent = new ByteArrayContent(Array.Empty<byte>());
        byteContent.Headers.ContentType = new MediaTypeHeaderValue($"video/{dto.Extension}");
        content.Add(byteContent, "Content", "SomeFileName.webp");
        var actual = await TestsHelpers.GetResultParsedFromJson<ResourcesDTO.ReadVideoDTO>(client,
                                                                                           "api/v1/videos",
                                                                                           bearer,
                                                                                           HttpMethod.Post,
                                                                                           _outputHelper,
                                                                                           content);
        Assert.Equal(dto.Name, actual.Name);
        Assert.Equal(dto.Duration, actual.Duration);
        Assert.Equal(dto.Extension, actual.Extension);
        Assert.Equal(dto.Tags, actual.Tags);
        client.Dispose();
    }


    [Fact]
    public async Task CreateNewVideo_WithNonexistentUser_ShouldReturnBadRequest()
    {
        var dto = new ResourcesDTO.CreateVideoDTO()
                  {
                      Content = new FormFile(Stream.Null, 0, 0, "SomeName", "FileName"),
                      Duration = 10,
                      Extension = "webp",
                      Name = "Some video name",
                      Tags = new[] {"hello", "best", "dog"},
                      OwnerId = PostgresqlCollectItDbContext.DefaultUsers.Max(u => u.Id) + 1,
                      UploadDate = DateTime.UtcNow
                  };
        var (client, bearer) = await Initialize();
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(dto.Name), "Name");
        content.Add(new StringContent(dto.UploadDate.ToString()), "UploadDate");
        content.Add(new StringContent(dto.Extension), "Extension");
        content.Add(new StringContent(dto.Duration.ToString()), "Duration");
        content.Add(new StringContent(dto.OwnerId.ToString()), "OwnerId");
        dto.Tags.ToList().ForEach(t => content.Add(new StringContent(t), "Tags"));
        var byteContent = new ByteArrayContent(Array.Empty<byte>());
        byteContent.Headers.ContentType = new MediaTypeHeaderValue($"video/{dto.Extension}");
        content.Add(byteContent, "Content", "SomeFileName.webp");
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 "api/v1/videos", 
                                                 HttpStatusCode.NotFound, 
                                                 HttpMethod.Post,
                                                 bearer,
                                                 content);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteVideo_WithValidId_ShouldDeleteVideo()
    {
        var (client, bearer) = await Initialize();
        var video = DefaultVideos.First();
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 $"api/v1/videos/{video.Id}", 
                                                 HttpStatusCode.NoContent,
                                                 HttpMethod.Delete, bearer);
        await TestsHelpers.AssertStatusCodeAsync(client, 
                                                 $"api/v1/videos/{video.Id}", 
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Get, bearer);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteVideo_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        var id = DefaultVideos.Max(v => v.Id) + 1;
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{id}",
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Delete, bearer);
        client.Dispose();
    }

    [Fact]
    public async Task DownloadVideoContent_WithValidId_ShouldReturnContent()
    {
        var (client, bearer) = await Initialize();
        var video = DefaultVideos.First();
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{video.Id}/download",
                                                 HttpStatusCode.OK,
                                                 HttpMethod.Get, bearer);
        client.Dispose();
    }
    
    
    
    [Fact]
    public async Task DownloadVideoContent_WithUserDidNotAcquireVideo_ShouldReturnPaymentRequired()
    {
        var (client, bearer) = await Initialize(PostgresqlCollectItDbContext.DefaultUserOne.UserName, "12345678");
        var video = DefaultVideos.First();
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{video.Id}/download",
                                                 HttpStatusCode.PaymentRequired,
                                                 HttpMethod.Get, bearer);
        client.Dispose();
    }
    
    [Fact]
    public async Task DownloadVideoContent_WithInvalidId_ShouldReturnNotFound()
    {
        var (client, bearer) = await Initialize();
        var id = DefaultVideos.Max(v => v.Id) + 1;
        await TestsHelpers.AssertStatusCodeAsync(client,
                                                 $"api/v1/videos/{id}/download",
                                                 HttpStatusCode.NotFound,
                                                 HttpMethod.Get, bearer);
        client.Dispose();
    }
    
    
    private static void AssertVideosEqual(Video video, ResourcesDTO.ReadVideoDTO dto)
    {
        Assert.Equal(video.Id, dto.Id);
        Assert.Equal(video.Name, dto.Name);
        Assert.Equal(video.Duration, dto.Duration);
        Assert.Equal(video.Extension, dto.Extension);
        Assert.Equal(video.OwnerId, dto.OwnerId);
        Assert.Equal(video.Tags, dto.Tags);
    }
}