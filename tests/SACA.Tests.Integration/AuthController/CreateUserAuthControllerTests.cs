using Bogus;
using FluentAssertions;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

public class CreateUserAuthControllerTests : IClassFixture<TestFactory>
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


    public CreateUserAuthControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }
    
    [Fact]
    public async Task<UserResponse> Should_CreateAndDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        var user = _faker.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync($"v2/Auth/signup", user);
        // var requestMessage = new HttpRequestMessage
        // {
        //     Method = HttpMethod.Post,
        //     RequestUri = new Uri($"v2/Auth/signup", UriKind.Relative),
        //     Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"),
        //
        // };
        //
        // var response = await _client.SendAsync(requestMessage);
        
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_testFactory);
        var deletedUser = await deleteUserAuthControllerTests.Should_DeleteUser_WhenUserExist(userResponse.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Username.Should().Equals(user.UserName);
        deletedUser.Id.Should().Equals(userResponse.Id);

        return userResponse;
    }
    
    [Fact]
    public async Task<UserResponse> Should_CreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        var user = _faker.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync($"v2/Auth/signup", user);
        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userResponse.Username.Should().Equals(user.UserName);

        return userResponse;
    }
}