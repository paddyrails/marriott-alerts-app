namespace Domain.Entities;

public class AlertDef
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AwsAccountId { get; set; } = string.Empty;
    public int MaxBillAmount { get; set; }
    public string AlertRecipientEmails { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
