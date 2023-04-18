using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.UsersController;

public class DeleteUserAuthControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public DeleteUserAuthControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    public async Task<UserResponse> Should_DeleteUser_WhenUserExist(int id)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.DeleteAsync("v2/Users");
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Be(id);

        return userResponse;
    }
    
    [Theory]
    [InlineData(2)]
    public async Task Should_ReturnForbidden_WhenUserIsNotSuperuser(int id)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.DeleteAsync($"v2/Users/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}