using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IAlertDefRepository
{
    Task<AlertDef?> GetByIdAsync(Guid id);
    Task<(List<AlertDef> Items, int Total)> GetByUserIdAsync(Guid userId, int page, int limit);
    Task<AlertDef> CreateAsync(AlertDef alertDef);
    Task<AlertDef> UpdateAsync(AlertDef alertDef);
    Task DeleteAsync(AlertDef alertDef);
}
