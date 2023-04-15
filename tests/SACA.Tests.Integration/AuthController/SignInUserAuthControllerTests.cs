using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

public class SignInUserAuthControllerTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _authApiFactory;
    private readonly HttpClient _client;

    public SignInUserAuthControllerTests(AuthApiFactory authApiFactory)
    {
        _authApiFactory = authApiFactory;
        _client = authApiFactory.CreateClient();
    }
    
    public async Task<UserResponse> Should_SignIn_WhenUserExist(int id)
    {
        // Arrange
        var getUserAuthControllerTests = new GetUserAuthControllerTests(_authApiFactory);
        var user = await getUserAuthControllerTests.Should_GetUser_WhenUserExist(id);

        // Act
        var response = await _client.PostAsJsonAsync($"v2/auth/signin", new SignInRequest
        {
            Email = user.Email,
            Password = "123qwe",
            Remember = false,
        });
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Equals(user.Id);

        return userResponse;
    }
}