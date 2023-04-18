using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SACA.Tests.Integration.ImagesController;

public class CreateImageControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    private readonly Faker<ImageRequest> _faker = new Faker<ImageRequest>()
        .RuleFor(x => x.CategoryId, faker => 1)
        .RuleFor(x => x.Name, faker => faker.Person.FirstName)
        .RuleFor(x => x.Base64, faker => DatabaseConstants.CatCreate);


    public CreateImageControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }

    [Theory]
    [InlineData(1)]
    public async Task Should_CreateAndDeleteUserImage_WhenUserExists(int userId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);
        var deleteImageControllerTests = new DeleteImageControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        var createdImageResponse = await Should_CreateUserImage_WhenUserExists(userId);
        var deletedImageResponse = await deleteImageControllerTests.Should_DeleteUserImage_WhenUserExists(user.Id, createdImageResponse.Id);

        // Assert
        deletedImageResponse.Id.Should().Be(createdImageResponse.Id);
    }

    public async Task<ImageResponse> Should_CreateUserImage_WhenUserExists(int userId)
    {
        // Arrange
        var content = _faker.Generate();

        var signInUserAuthControllerTests = new SignInAuthControllerTests(_testFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        
        // var requestMessage = new HttpRequestMessage
        // {
        //     Method = HttpMethod.Post,
        //     RequestUri = new Uri($"v2/Images", UriKind.Relative),
        //     Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"),
        //     Headers =
        //     {
        //         Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
        //     },
        // };

        var response = await _client.PostAsJsonAsync("v2/Images", content);
        var imageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return imageResponse;
    }
}