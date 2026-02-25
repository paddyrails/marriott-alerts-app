using Api.Security;
using Application.AlertDefs;
using Application.AlertDefs.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertDefsController : ControllerBase
{
    private readonly IAlertDefService _alertDefService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<AlertDefCreateRequest> _createValidator;
    private readonly IValidator<AlertDefUpdateRequest> _updateValidator;

    public AlertDefsController(
        IAlertDefService alertDefService,
        ICurrentUserService currentUserService,
        IValidator<AlertDefCreateRequest> createValidator,
        IValidator<AlertDefUpdateRequest> updateValidator)
    {
        _alertDefService = alertDefService;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AlertDefCreateRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var result = await _alertDefService.CreateAsync(_currentUserService.UserId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        if (page < 1) page = 1;
        if (limit < 1) limit = 20;
        if (limit > 100) limit = 100;

        var result = await _alertDefService.GetAllAsync(_currentUserService.UserId, page, limit);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _alertDefService.GetByIdAsync(_currentUserService.UserId, id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlertDefUpdateRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var result = await _alertDefService.UpdateAsync(_currentUserService.UserId, id, request);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _alertDefService.DeleteAsync(_currentUserService.UserId, id);
        return NoContent();
    }
}
