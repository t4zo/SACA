using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.ImagesController;

public class GetImagesControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public GetImagesControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Should_GetUserImages_WhenUserExists(int userId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        
        var response = await _client.GetAsync("v2/Images");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(1, 100)]
    public async Task<ImageResponse> Should_GetImage_WhenIsUserImage(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.GetAsync($"v2/Images/{imageId}");
        var imageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        imageResponse.Id.Should().Be(imageId);

        return imageResponse;
    }
    
    [Theory]
    [InlineData(1, 1)]
    public async Task Should_ReturnNotFound_WhenIsNotUserImage(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.GetAsync($"v2/Images/{imageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnsUnauthorizedResult_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("v2/Images");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}