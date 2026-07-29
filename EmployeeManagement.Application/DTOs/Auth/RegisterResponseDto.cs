namespace EmployeeManagement.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public int user_id { get; set; }

    public string first_name { get; set; } = string.Empty;

    public string last_name { get; set; } = string.Empty;

    public string email { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public List<string> roles { get; set; } = new();
}