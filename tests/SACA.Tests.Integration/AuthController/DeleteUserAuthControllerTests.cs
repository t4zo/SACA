using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

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
        var signInUserAuthControllerTests = new SignInUserAuthControllerTests(_testFactory);
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(id);
        
        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"v2/Auth/user", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Equals(id);

        return userResponse;
    }
    
    [Fact]
    public async Task Should_CreateAndDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        var createUserAuthControllerTests = new CreateUserAuthControllerTests(_testFactory);
        var user = await createUserAuthControllerTests.Should_CreateUser_WhenUserDoesNotExist();
        
        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"v2/Auth/user", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Id.Should().Equals(user.Id);
    }
}