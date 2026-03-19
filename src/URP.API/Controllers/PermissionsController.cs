using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URP.Application.Common;
using URP.Application.DTOs.Permissions;
using URP.Application.Interfaces;
using URP.Infrastructure.DependencyInjection;

namespace URP.API.Controllers;

/// <summary>Permission management and role-permission assignment.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class PermissionsController(
    IPermissionService permService,
    IValidator<CreatePermissionRequest> validator) : ControllerBase
{
    /// <summary>Get all permissions. Filter by group: ?group=Users. Requires permissions:read.</summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.PermissionsRead)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionResponse>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? group, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<PermissionResponse>>.Ok(await permService.GetAllAsync(group, ct)));

    /// <summary>Get a permission by ID. Requires permissions:read.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PolicyNames.PermissionsRead)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => Ok(ApiResponse<PermissionResponse>.Ok(await permService.GetByIdAsync(id, ct)));

    /// <summary>Create a permission. Name format: resource:action. Requires permissions:create.</summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.PermissionsCreate)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest req, CancellationToken ct)
    {
        var v = await validator.ValidateAsync(req, ct);
        if (!v.IsValid)
            return BadRequest(ApiResponse.Fail("Validation failed", v.Errors.Select(e => e.ErrorMessage).ToList()));

        var perm = await permService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = perm.Id },
            ApiResponse<PermissionResponse>.Ok(perm, "Permission created"));
    }

    /// <summary>Assign a permission to a role. Body: { "roleId": 2, "permissionId": 5 }. Requires permissions:assign.</summary>
    [HttpPost("assign")]
    [Authorize(Policy = PolicyNames.PermissionsAssign)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Assign([FromBody] AssignPermissionRequest req, CancellationToken ct)
    {
        await permService.AssignToRoleAsync(req, ct);
        return Ok(ApiResponse.Ok("Permission assigned to role successfully"));
    }

    /// <summary>Remove a permission from a role. Body: { "roleId": 2, "permissionId": 5 }. Requires permissions:assign.</summary>
    [HttpDelete("remove")]
    [Authorize(Policy = PolicyNames.PermissionsAssign)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Remove([FromBody] AssignPermissionRequest req, CancellationToken ct)
    {
        await permService.RemoveFromRoleAsync(req, ct);
        return Ok(ApiResponse.Ok("Permission removed from role"));
    }

    /// <summary>Get permissions for a specific role. Requires permissions:read.</summary>
    [HttpGet("role/{roleId:int}")]
    [Authorize(Policy = PolicyNames.PermissionsRead)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionResponse>>), 200)]
    public async Task<IActionResult> GetByRole(int roleId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<PermissionResponse>>.Ok(await permService.GetByRoleAsync(roleId, ct)));
}
