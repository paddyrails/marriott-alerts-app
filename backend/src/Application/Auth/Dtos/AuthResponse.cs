namespace Application.Auth.Dtos;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public AuthUserDto User { get; set; } = null!;
}

public class AuthUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
}
