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

public class GetAllImagesControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    private readonly Faker<ImageRequest> _faker = new Faker<ImageRequest>()
        .RuleFor(x => x.CategoryId, faker => 1)
        .RuleFor(x => x.Name, faker => faker.Person.FirstName)
        .RuleFor(x => x.Base64, faker => faker.Image.LoremPixelUrl());


    public GetAllImagesControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    [Theory]
    [InlineData(1, 100)]
    public async Task Should_GetImage_WhenIsUserImage(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInUserAuthControllerTests(_testFactory);
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        
        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v2/Images/{imageId}", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Equals(imageId);
    }
    
    [Theory]
    [InlineData(1, 1)]
    public async Task Should_ReturnNotFound_WhenIsNotUserImage(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInUserAuthControllerTests(_testFactory);
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        
        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v2/Images/{imageId}", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnsUnauthorizedResult_WhenUserIsPassed()
    {
        // Act
        var response = await _client.GetAsync("v2/Images");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}