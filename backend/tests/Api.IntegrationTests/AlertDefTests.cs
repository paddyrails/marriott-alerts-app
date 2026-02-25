using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Api.IntegrationTests;

public class AlertDefTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AlertDefTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndGetTokenAsync(string? email = null)
    {
        email ??= $"user-{Guid.NewGuid()}@example.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "password123",
            name = "Test User"
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("accessToken").GetString()!;
    }

    private HttpRequestMessage CreateAuthorized(HttpMethod method, string url, string token, object? content = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content is not null)
            request.Content = JsonContent.Create(content);
        return request;
    }

    [Fact]
    public async Task Post_AlertDef_WithoutToken_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/AlertDefs", new
        {
            name = "Test Alert",
            awsAccountId = "123456789012",
            maxBillAmount = 100,
            alertRecipientEmails = "test@example.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_AlertDef_WithToken_ReturnsCreated()
    {
        var token = await RegisterAndGetTokenAsync();

        var request = CreateAuthorized(HttpMethod.Post, "/api/AlertDefs", token, new
        {
            name = "Test Alert",
            awsAccountId = "123456789012",
            maxBillAmount = 100,
            alertRecipientEmails = "test@example.com"
        });

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("name").GetString().Should().Be("Test Alert");
        body.GetProperty("id").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Get_AlertDefs_ReturnsUserScopedList()
    {
        var token = await RegisterAndGetTokenAsync();

        var createReq = CreateAuthorized(HttpMethod.Post, "/api/AlertDefs", token, new
        {
            name = "My Alert",
            awsAccountId = "111111111111",
            maxBillAmount = 50,
            alertRecipientEmails = "me@example.com"
        });
        await _client.SendAsync(createReq);

        var listReq = CreateAuthorized(HttpMethod.Get, "/api/AlertDefs", token);
        var response = await _client.SendAsync(listReq);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("items").GetArrayLength().Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task UserA_CannotAccess_UserB_AlertDef()
    {
        var tokenA = await RegisterAndGetTokenAsync();
        var tokenB = await RegisterAndGetTokenAsync();

        var createReq = CreateAuthorized(HttpMethod.Post, "/api/AlertDefs", tokenA, new
        {
            name = "User A Alert",
            awsAccountId = "222222222222",
            maxBillAmount = 200,
            alertRecipientEmails = "a@example.com"
        });
        var createResponse = await _client.SendAsync(createReq);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var alertDefId = createdBody.GetProperty("id").GetString();

        var getReq = CreateAuthorized(HttpMethod.Get, $"/api/AlertDefs/{alertDefId}", tokenB);
        var getResponse = await _client.SendAsync(getReq);

        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Patch_AlertDef_UpdatesFields()
    {
        var token = await RegisterAndGetTokenAsync();

        var createReq = CreateAuthorized(HttpMethod.Post, "/api/AlertDefs", token, new
        {
            name = "Before Update",
            awsAccountId = "333333333333",
            maxBillAmount = 300,
            alertRecipientEmails = "before@example.com"
        });
        var createResponse = await _client.SendAsync(createReq);
        var body = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetString();

        var patchReq = CreateAuthorized(HttpMethod.Patch, $"/api/AlertDefs/{id}", token, new
        {
            name = "After Update",
            maxBillAmount = 999
        });
        var patchResponse = await _client.SendAsync(patchReq);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<JsonElement>();
        updated.GetProperty("name").GetString().Should().Be("After Update");
        updated.GetProperty("maxBillAmount").GetInt32().Should().Be(999);
    }

    [Fact]
    public async Task Delete_AlertDef_Returns204()
    {
        var token = await RegisterAndGetTokenAsync();

        var createReq = CreateAuthorized(HttpMethod.Post, "/api/AlertDefs", token, new
        {
            name = "To Delete",
            awsAccountId = "444444444444",
            maxBillAmount = 400,
            alertRecipientEmails = "delete@example.com"
        });
        var createResponse = await _client.SendAsync(createReq);
        var body = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetString();

        var deleteReq = CreateAuthorized(HttpMethod.Delete, $"/api/AlertDefs/{id}", token);
        var deleteResponse = await _client.SendAsync(deleteReq);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getReq = CreateAuthorized(HttpMethod.Get, $"/api/AlertDefs/{id}", token);
        var getResponse = await _client.SendAsync(getReq);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
