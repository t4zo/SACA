using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

public class SignInUserAuthControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public SignInUserAuthControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    public async Task<UserResponse> Should_SignIn_WhenUserExist(int id)
    {
        // Arrange
        var getUserAuthControllerTests = new GetUserAuthControllerTests(_testFactory);

        // Act
        var user = await getUserAuthControllerTests.Should_GetUser_WhenUserExist(id);
        var response = await _client.PostAsJsonAsync($"v2/auth/signin", new SignInRequest
        {
            Email = user.Email,
            Password = "123qwe",
            Remember = false,
        });
        
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Be(user.Id);

        return userResponse;
    }
}