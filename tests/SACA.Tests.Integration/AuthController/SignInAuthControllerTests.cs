using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.UsersController;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SACA.Tests.Integration.AuthController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class SignInAuthControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public SignInAuthControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }
    
    public async Task<UserResponse> Should_SignInUser_WhenUserExist(int id)
    {
        // Arrange
        var getUserAuthControllerTests = new GetUserAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await getUserAuthControllerTests.Should_ReturnUser_WhenUserExist(id);
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

    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}