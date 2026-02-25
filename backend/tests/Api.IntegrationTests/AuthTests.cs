using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Api.IntegrationTests;

public class AuthTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsAccessToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"test-{Guid.NewGuid()}@example.com",
            password = "password123",
            name = "Test User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("user").GetProperty("email").GetString().Should().Contain("@example.com");
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var email = $"dup-{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });

        var response = await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "not-an-email",
            password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShortPassword_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = $"test-{Guid.NewGuid()}@example.com",
            password = "short"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        var email = $"login-{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "password123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var email = $"bad-{Guid.NewGuid()}@example.com";
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "wrongpassword" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
