using Bogus;
using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.UsersController;
using System.Net;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class SignUpAuthControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    private readonly Faker<SignUpRequest> _faker = new Faker<SignUpRequest>()
        .RuleFor(x => x.UserName, faker => faker.Person.UserName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.Password, _ => "123qwe")
        .RuleFor(x => x.ConfirmPassword, _ => "123qwe")
        .RuleFor(x => x.Roles, _ => new List<string> { "User" });


    public SignUpAuthControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }
    
    [Fact]
    public async Task Should_CreateAndDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_integrationTestFactory);
        
        // Act
        var createdUser = await Should_SignUpUser_WhenUserDoesNotExist();
        var deletedUser = await deleteUserAuthControllerTests.Should_DeleteUser_WhenUserExist(createdUser.Id);

        // Assert
        deletedUser.Id.Should().Be(createdUser.Id);
    }

    public async Task<UserResponse> Should_SignUpUser_WhenUserDoesNotExist()
    {
        // Arrange
        var user = _faker.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync($"v2/Auth/signup", user);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return userResponse;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}