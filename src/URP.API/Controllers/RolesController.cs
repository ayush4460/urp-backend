using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URP.Application.Common;
using URP.Application.DTOs.Roles;
using URP.Application.DTOs.Users;
using URP.Application.Interfaces;
using URP.Infrastructure.DependencyInjection;

namespace URP.API.Controllers;

/// <summary>Role management and user-role assignment.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class RolesController(
    IRoleService roleService,
    IValidator<CreateRoleRequest> validator,
    ILogger<RolesController> logger) : ControllerBase
{
    /// <summary>Get all roles with their permissions. Requires roles:read.</summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.RolesRead)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponse>>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<RoleResponse>>.Ok(await roleService.GetAllAsync(ct)));

    /// <summary>Get a role by ID. Requires roles:read.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = PolicyNames.RolesRead)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => Ok(ApiResponse<RoleResponse>.Ok(await roleService.GetByIdAsync(id, ct)));

    /// <summary>Create a new role. Requires roles:create.</summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.RolesCreate)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest req, CancellationToken ct)
    {
        var v = await validator.ValidateAsync(req, ct);
        if (!v.IsValid)
            return BadRequest(ApiResponse.Fail("Validation failed", v.Errors.Select(e => e.ErrorMessage).ToList()));

        var role = await roleService.CreateAsync(req, ct);
        logger.LogInformation("Role created: {Name}", role.Name);
        return CreatedAtAction(nameof(GetById), new { id = role.Id },
            ApiResponse<RoleResponse>.Ok(role, "Role created successfully"));
    }

    /// <summary>Update a role. Requires roles:update.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyNames.RolesUpdate)]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateRoleRequest req, CancellationToken ct)
        => Ok(ApiResponse<RoleResponse>.Ok(await roleService.UpdateAsync(id, req, ct), "Role updated"));

    /// <summary>Delete a role. Requires roles:delete.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = PolicyNames.RolesDelete)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await roleService.DeleteAsync(id, ct);
        return Ok(ApiResponse.Ok("Role deleted"));
    }

    /// <summary>Assign a role to a user. Body: { "userId": 5, "roleId": 2 }. Requires roles:assign.</summary>
    [HttpPost("assign")]
    [Authorize(Policy = PolicyNames.RolesAssign)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Assign([FromBody] AssignRoleRequest req, CancellationToken ct)
    {
        await roleService.AssignToUserAsync(req, ct);
        return Ok(ApiResponse.Ok("Role assigned to user successfully"));
    }

    /// <summary>Remove a role from a user. Body: { "userId": 5, "roleId": 2 }. Requires roles:assign.</summary>
    [HttpDelete("remove")]
    [Authorize(Policy = PolicyNames.RolesAssign)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Remove([FromBody] RemoveRoleRequest req, CancellationToken ct)
    {
        await roleService.RemoveFromUserAsync(req, ct);
        return Ok(ApiResponse.Ok("Role removed from user"));
    }

    /// <summary>Get all users in a role. Requires roles:read.</summary>
    [HttpGet("{id:int}/users")]
    [Authorize(Policy = PolicyNames.RolesRead)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponse>>), 200)]
    public async Task<IActionResult> GetUsers(int id, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<UserResponse>>.Ok(await roleService.GetUsersInRoleAsync(id, ct)));
}
