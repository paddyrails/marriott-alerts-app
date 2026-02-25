namespace Application.AlertDefs.Dtos;

public class AlertDefUpdateRequest
{
    public string? Name { get; set; }
    public string? AwsAccountId { get; set; }
    public int? MaxBillAmount { get; set; }
    public string? AlertRecipientEmails { get; set; }
}
