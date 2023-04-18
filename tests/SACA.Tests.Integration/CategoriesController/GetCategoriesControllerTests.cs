using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities;
using SACA.Tests.Integration.AuthController;
using SACA.Tests.Integration.UsersController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SACA.Tests.Integration.CategoriesController;

public class GetCategoriesControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public GetCategoriesControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }

    [Fact]
    public async Task Should_ReturnCategories_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("v2/Categories");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Should_ReturnCategories_WhenUserIsAuthenticated(int userId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.GetAsync("v2/Categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Should_ReturnCategory_WhenCategoryExist(int id)
    {
        // Arrange
        var createUserAuthControllerTests = new SignUpAuthControllerTests(_testFactory);
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_testFactory);

        // Act
        var user = await createUserAuthControllerTests.Should_SignUpUser_WhenUserDoesNotExist();
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v2/Categories/{id}", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };

        var response = await _client.SendAsync(requestMessage);
        var categoryResponse = await response.Content.ReadFromJsonAsync<Category>();
        
        await deleteUserAuthControllerTests.Should_DeleteUser_WhenUserExist(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        categoryResponse.Id.Should().Be(id);
    }
}
