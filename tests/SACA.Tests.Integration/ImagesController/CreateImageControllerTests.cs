using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SACA.Tests.Integration.ImagesController;

[Collection(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class CreateImageControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFactory _integrationTestFactory;
    private readonly HttpClient _client;

    private readonly Faker<ImageRequest> _faker = new Faker<ImageRequest>()
        .RuleFor(x => x.CategoryId, faker => 1)
        .RuleFor(x => x.Name, faker => faker.Person.FirstName)
        .RuleFor(x => x.Base64, faker => DatabaseConstants.CatCreate);


    public CreateImageControllerTests(IntegrationTestFactory integrationTestFactory)
    {
        _integrationTestFactory = integrationTestFactory;
        _client = integrationTestFactory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = default;
    }

    [Theory]
    [InlineData(1)]
    public async Task Should_CreateAndDeleteUserImage_WhenUserExists(int userId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);
        var deleteImageControllerTests = new DeleteImageControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(userId);
        var createdImageResponse = await Should_CreateUserImage_WhenUserExists(userId);
        var deletedImageResponse = await deleteImageControllerTests.Should_DeleteUserImage_WhenUserExists(user.Id, createdImageResponse.Id);

        // Assert
        deletedImageResponse.Id.Should().Be(createdImageResponse.Id);
    }

    public async Task<ImageResponse> Should_CreateUserImage_WhenUserExists(int userId)
    {
        // Arrange
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "gato.jpg");
        // await using var file1 = File.OpenRead(filePath);
        // using var content1 = new StreamContent(file1);
        
        // Other ways to create stream
        using var stream = new MemoryStream();
        
        // Memory file
        // var bytes = Convert.FromBase64String(DatabaseConstants.CatCreate);
        
        // File path
        var bytes = Encoding.UTF8.GetBytes(filePath);
        
        stream.Write(bytes);
        using var content1 = new StreamContent(stream);

        // Optional header
        // content1.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        // content1.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        
        using var formData = new MultipartFormDataContent
        {
            {content1, "file", "gato.png"},
            { new StringContent("1"), "categoryId" },
            // { new StringContent("110"), "resizeWidth" },
            // { new StringContent("150"), "resizeHeight" },
            // { new StringContent("false"), "compress" },
        };

        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(userId);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        // _client.DefaultRequestHeaders.TryAddWithoutValidation("categoryId", "1");

        // Optional headers
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
        // // fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");


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

        var response = await _client.PostAsync("v2/Images", formData);
        var imageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return imageResponse;
    }

    [Theory]
    [InlineData(1)]
    public async Task Should_CreateAndDeleteImage_WhenUserIsSuperuser(int id)
    {
        // Arrange
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "gato.jpg");
        using var stream = new MemoryStream();
        var bytes = Encoding.UTF8.GetBytes(filePath);
        stream.Write(bytes);
        using var content1 = new StreamContent(stream);
        
        using var formData = new MultipartFormDataContent
        {
            {content1, "file", "gato.png"},
            { new StringContent("2"), "categoryId" },
            // { new StringContent("110"), "resizeWidth" },
            // { new StringContent("150"), "resizeHeight" },
            // { new StringContent("false"), "compress" },
        };

        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);
        // _client.DefaultRequestHeaders.TryAddWithoutValidation("categoryId", "2");

        var createImage = await _client.PostAsync("v2/Images/superuser", formData);
        var createImageResponse = await createImage.Content.ReadFromJsonAsync<ImageResponse>();

        var deleteImage = await _client.DeleteAsync($"v2/Images/superuser/{createImageResponse.Id}");
        var deleteImageResponse = await deleteImage.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        createImage.StatusCode.Should().Be(HttpStatusCode.OK);
        deleteImage.StatusCode.Should().Be(HttpStatusCode.OK);

        createImageResponse.Id.Should().Be(deleteImageResponse.Id);
    }

    [Theory]
    [InlineData(2)]
    public async Task Should_ReturnForbidden_WhenUserIsNotSuperuser(int id)
    {
        // Arrange
        var content = _faker.Generate();

        var signInUserAuthControllerTests = new SignInAuthControllerTests(_integrationTestFactory);

        // Act
        var user = await signInUserAuthControllerTests.Should_SignInUser_WhenUserExist(id);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token);

        var response = await _client.PostAsJsonAsync("v2/Images/superuser", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _integrationTestFactory.ResetDatabaseAsync();
}