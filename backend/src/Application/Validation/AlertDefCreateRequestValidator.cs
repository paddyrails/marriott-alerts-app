using Application.AlertDefs.Dtos;
using FluentValidation;

namespace Application.Validation;

public class AlertDefCreateRequestValidator : AbstractValidator<AlertDefCreateRequest>
{
    public AlertDefCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.AwsAccountId)
            .NotEmpty().WithMessage("AWS Account ID is required.");

        RuleFor(x => x.MaxBillAmount)
            .GreaterThan(0).WithMessage("Max bill amount must be greater than 0.");

        RuleFor(x => x.AlertRecipientEmails)
            .NotEmpty().WithMessage("Alert recipient emails are required.");
    }
}
