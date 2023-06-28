using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Entities.Identity;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SACA.Tests.Integration.UsersController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class GetUserAuthControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    public GetUserAuthControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }

    [Theory]
    [InlineData(1)]
    public async Task<List<UserResponse>> Should_ReturnUsers_WhenUserIsSuperuser(int id)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        
        // Act
        var response = await _client.GetAsync($"v2/Users");
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return users;
    }
    
    [Theory]
    [InlineData(2)]
    public async Task Should_ReturnForbidden_WhenIsNotSuperuser(int id)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        
        // Act
        var response = await _client.GetAsync($"v2/Users");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task<ApplicationUser> Should_ReturnUser_WhenUserExist(int id)
    {
        // Act
        var user = await _client.GetFromJsonAsync<ApplicationUser>($"v2/Users/{id}");

        // Assert
        user.Id.Should().Be(id);

        return user;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}