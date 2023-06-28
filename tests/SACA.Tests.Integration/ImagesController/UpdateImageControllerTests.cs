using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SACA.Tests.Integration.ImagesController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class UpdateImageControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public UpdateImageControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }

    [Theory]
    [InlineData(1, 19)]
    public async Task<ImageResponse> Should_UpdateUserImage_WhenUserExists(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);
        var getImagesControllerTests = new GetImagesControllerTests(_integrationTestFactory);
        
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(userId);
        var image = await getImagesControllerTests.Should_ReturnImage_WhenIsUserImage(userId, imageId);

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "gato.jpg");
        using var stream = new MemoryStream();
        var bytes = Encoding.UTF8.GetBytes(filePath);
        stream.Write(bytes);
        using var content1 = new StreamContent(stream);
        
        using var formData = new MultipartFormDataContent
        {
            {content1, "file", image.Name},
        };
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        _client.DefaultRequestHeaders.TryAddWithoutValidation("categoryId", "1");
        // _client.DefaultRequestHeaders.TryAddWithoutValidation("resizeWidth", "110");
        // _client.DefaultRequestHeaders.TryAddWithoutValidation("resizeHeight", "150");
        // _client.DefaultRequestHeaders.TryAddWithoutValidation("compress", "false");
        
        var response = await _client.PutAsync($"v2/Images/{imageId}", formData);
        var updatedImageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedImageResponse.Id.Should().Be(imageId);

        return updatedImageResponse;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}