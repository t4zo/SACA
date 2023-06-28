using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SACA.Tests.Integration.UsersController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class DeleteUserAuthControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public DeleteUserAuthControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }
    
    public async Task<UserResponse> Should_DeleteUser_WhenUserExist(int id)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.DeleteAsync("v2/Auth/user");
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
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.DeleteAsync($"v2/Auth/user/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}