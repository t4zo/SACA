using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using SACA.Entities;
using SACA.Tests.Integration.AuthController;
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
    public async Task Should_GetAllCategories_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("v2/Categories");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Should_GetAllCategories_WhenUserIsAuthenticated(int userId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInUserAuthControllerTests(_testFactory);
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"v2/Categories", UriKind.Relative),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Should_GetCategory_WhenCategoryExist(int id)
    {
        // Arrange
        var createUserAuthControllerTests = new CreateUserAuthControllerTests(_testFactory);
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_testFactory);

        // Act
        var user = await createUserAuthControllerTests.Should_CreateUser_WhenUserDoesNotExist();
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
