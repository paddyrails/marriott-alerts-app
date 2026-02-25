using Application.AlertDefs.Dtos;
using Application.Auth;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.AlertDefs;

public interface IAlertDefService
{
    Task<AlertDefResponse> CreateAsync(Guid userId, AlertDefCreateRequest request);
    Task<AlertDefListResponse> GetAllAsync(Guid userId, int page, int limit);
    Task<AlertDefResponse> GetByIdAsync(Guid userId, Guid id);
    Task<AlertDefResponse> UpdateAsync(Guid userId, Guid id, AlertDefUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid id);
}

public class AlertDefService : IAlertDefService
{
    private readonly IAlertDefRepository _alertDefRepository;

    public AlertDefService(IAlertDefRepository alertDefRepository)
    {
        _alertDefRepository = alertDefRepository;
    }

    public async Task<AlertDefResponse> CreateAsync(Guid userId, AlertDefCreateRequest request)
    {
        var alertDef = new AlertDef
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            AwsAccountId = request.AwsAccountId,
            MaxBillAmount = request.MaxBillAmount,
            AlertRecipientEmails = request.AlertRecipientEmails,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _alertDefRepository.CreateAsync(alertDef);
        return MapToResponse(alertDef);
    }

    public async Task<AlertDefListResponse> GetAllAsync(Guid userId, int page, int limit)
    {
        var (items, total) = await _alertDefRepository.GetByUserIdAsync(userId, page, limit);

        return new AlertDefListResponse
        {
            Items = items.Select(MapToResponse).ToList(),
            Page = page,
            Limit = limit,
            Total = total
        };
    }

    public async Task<AlertDefResponse> GetByIdAsync(Guid userId, Guid id)
    {
        var alertDef = await _alertDefRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"AlertDef with id '{id}' not found.");

        if (alertDef.UserId != userId)
            throw new ForbiddenException("You do not have access to this resource.");

        return MapToResponse(alertDef);
    }

    public async Task<AlertDefResponse> UpdateAsync(Guid userId, Guid id, AlertDefUpdateRequest request)
    {
        var alertDef = await _alertDefRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"AlertDef with id '{id}' not found.");

        if (alertDef.UserId != userId)
            throw new ForbiddenException("You do not have access to this resource.");

        if (request.Name is not null) alertDef.Name = request.Name;
        if (request.AwsAccountId is not null) alertDef.AwsAccountId = request.AwsAccountId;
        if (request.MaxBillAmount.HasValue) alertDef.MaxBillAmount = request.MaxBillAmount.Value;
        if (request.AlertRecipientEmails is not null) alertDef.AlertRecipientEmails = request.AlertRecipientEmails;
        alertDef.UpdatedAt = DateTimeOffset.UtcNow;

        await _alertDefRepository.UpdateAsync(alertDef);
        return MapToResponse(alertDef);
    }

    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var alertDef = await _alertDefRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"AlertDef with id '{id}' not found.");

        if (alertDef.UserId != userId)
            throw new ForbiddenException("You do not have access to this resource.");

        await _alertDefRepository.DeleteAsync(alertDef);
    }

    private static AlertDefResponse MapToResponse(AlertDef alertDef) => new()
    {
        Id = alertDef.Id,
        Name = alertDef.Name,
        AwsAccountId = alertDef.AwsAccountId,
        MaxBillAmount = alertDef.MaxBillAmount,
        AlertRecipientEmails = alertDef.AlertRecipientEmails,
        CreatedAt = alertDef.CreatedAt,
        UpdatedAt = alertDef.UpdatedAt
    };
}
