using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SACA.Tests.Integration.ImagesController;

public class DeleteImageControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public DeleteImageControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    public async Task<ImageResponse> Should_DeleteUserImage_WhenUserExists(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.DeleteAsync($"v2/Images/{imageId}");
        var deletedImageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        deletedImageResponse.Id.Should().Be(imageId);
        
        return deletedImageResponse;
    }
}