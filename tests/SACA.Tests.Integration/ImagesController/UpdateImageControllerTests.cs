using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SACA.Tests.Integration.ImagesController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class UpdateImageControllerTests 
    // : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public UpdateImageControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
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

        var content = new ImageRequest
        {
            Id = image.Id,
            CategoryId = image.CategoryId,
            Name = image.Name,
            Base64 = DatabaseConstants.CatUpdate,
        };
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        
        var response = await _client.PutAsJsonAsync($"v2/Images/{imageId}", content);
        var updatedImageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedImageResponse.Id.Should().Be(imageId);

        return updatedImageResponse;
    }
    
    // public Task InitializeAsync() => Task.CompletedTask;
    //
    // public async Task DisposeAsync() => await _testFactory.ResetDatabaseAsync();
}