using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Tests.Integration.AuthController;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SACA.Tests.Integration.ImagesController;

public class UpdateImageControllerTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;
    private readonly HttpClient _client;

    public UpdateImageControllerTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
        _client = testFactory.CreateClient();
    }

    [Theory]
    [InlineData(1, 100)]
    public async Task<ImageResponse> Should_UpdateUserImage_WhenIsUserExists(int userId, int imageId)
    {
        // Arrange
        var signInUserAuthControllerTests = new SignInUserAuthControllerTests(_testFactory);
        var getImagesControllerTests = new GetImagesControllerTests(_testFactory);
        
        // Act
        var user = await signInUserAuthControllerTests.Should_SignIn_WhenUserExist(userId);
        var image = await getImagesControllerTests.Should_GetImage_WhenUserImage(userId, imageId);

        var content = new ImageRequest
        {
            Id = image.Id,
            CategoryId = image.CategoryId,
            Name = image.Name,
            Base64 = DatabaseConstants.CatUpdate,
        };
        
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"v2/Images/{imageId}", UriKind.Relative),
            Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, user.Token),
            },
        };
        
        var response = await _client.SendAsync(requestMessage);
        var updatedImageResponse = await response.Content.ReadFromJsonAsync<ImageResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedImageResponse.Id.Should().Be(imageId);

        return updatedImageResponse;
    }
}