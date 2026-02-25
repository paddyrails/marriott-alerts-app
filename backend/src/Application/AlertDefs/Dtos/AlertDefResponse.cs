namespace Application.AlertDefs.Dtos;

public class AlertDefResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AwsAccountId { get; set; } = string.Empty;
    public int MaxBillAmount { get; set; }
    public string AlertRecipientEmails { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AlertDefListResponse
{
    public List<AlertDefResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
}
