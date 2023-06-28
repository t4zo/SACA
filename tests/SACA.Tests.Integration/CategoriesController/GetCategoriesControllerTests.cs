using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities;
using SACA.Tests.Integration.AuthController;
using SACA.Tests.Integration.UsersController;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SACA.Tests.Integration.CategoriesController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class GetCategoriesControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public GetCategoriesControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }

    [Fact]
    public async Task Should_ReturnCategories_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = default;
        
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
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);
        
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
        var createUserAuthControllerTests = new SignUpAuthControllerTests(_integrationTestFactory);
        var deleteUserAuthControllerTests = new DeleteUserAuthControllerTests(_integrationTestFactory);

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
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}
