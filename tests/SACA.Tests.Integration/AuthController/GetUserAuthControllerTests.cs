using FluentAssertions;
using SACA.Entities.Identity;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.AuthController;

public class GetUserAuthControllerTests : IClassFixture<TestFactory>
{
    private readonly HttpClient _client;

    public GetUserAuthControllerTests(TestFactory testFactory)
    {
        _client = testFactory.CreateClient();
    }

    [Theory]
    [InlineData(1)]
    public async Task<ApplicationUser> Should_GetUser_WhenUserExist(int id)
    {
        // Act
        var user = await _client.GetFromJsonAsync<ApplicationUser>($"v2/Auth/{id}");

        // Assert
        user.Id.Should().Equals(id);

        return user;
    }
}