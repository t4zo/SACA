using Bogus;
using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.UsersController;
using System.Net;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

public class SignUpAuthControllerTests : IClassFixture<TestFactory>
// [Collection("Test collection")]
// public class CreateUserAuthControllerTests
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    private readonly Faker<SignUpRequest> _faker = new Faker<SignUpRequest>()
        .RuleFor(x => x.UserName, faker => faker.Person.UserName)
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.Password, _ => "123qwe")
        .RuleFor(x => x.ConfirmPassword, _ => "123qwe")
        .RuleFor(x => x.Roles, _ => new List<string> { "User" });


    public SignUpAuthControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    [Fact]
    public async Task Should_CreateAndDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_testFactory);
        
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
}