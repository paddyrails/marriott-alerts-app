namespace Application.AlertDefs.Dtos;

public class AlertDefCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string AwsAccountId { get; set; } = string.Empty;
    public int MaxBillAmount { get; set; }
    public string AlertRecipientEmails { get; set; } = string.Empty;
}
