namespace EmployeeManagement.Application.DTOs.Auth;

public class EmailOtpDataDto
{
    public long EmailOtpId { get; set; }

    public int UserId { get; set; }

    public string OtpHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public int AttemptCount { get; set; }
}