using Application.AlertDefs.Dtos;
using FluentValidation;

namespace Application.Validation;

public class AlertDefUpdateRequestValidator : AbstractValidator<AlertDefUpdateRequest>
{
    public AlertDefUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.MaxBillAmount)
            .GreaterThan(0).WithMessage("Max bill amount must be greater than 0.")
            .When(x => x.MaxBillAmount.HasValue);
    }
}
