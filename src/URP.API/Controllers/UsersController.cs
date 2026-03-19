using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URP.API.Extensions;
using URP.Application.Common;
using URP.Application.DTOs.Auth;
using URP.Application.DTOs.Users;
using URP.Application.Interfaces;
using URP.Infrastructure.DependencyInjection;

namespace URP.API.Controllers;

/// <summary>User management and authentication.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class UsersController(
    IUserService userService,
    IValidator<CreateUserRequest> createValidator,
    IValidator<UpdateUserRequest> updateValidator,
    ILogger<UsersController> logger) : ControllerBase
{
    /// <summary>Register a new user account. No authentication required.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 409)]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        var v = await createValidator.ValidateAsync(req, ct);
        if (!v.IsValid)
            return BadRequest(ApiResponse.Fail("Validation failed", v.Errors.Select(e => e.ErrorMessage).ToList()));

        var result = await userService.RegisterAsync(req, ct);
        logger.LogInformation("User registered: {Email}", req.Email);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<UserResponse>.Ok(result, "User registered successfully"));
    }

    /// <summary>Authenticate and receive a JWT access token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await userService.LoginAsync(req, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful"));
    }

    /// <summary>Get all users with pagination, search and sort. Requires users:read.</summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.UsersRead)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserResponse>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true, CancellationToken ct = default)
    {
        var query = new PaginationQuery
        {
            Page = page, PageSize = pageSize,
            Search = search, SortBy = sortBy, SortDescending = sortDescending,
        };
        return Ok(ApiResponse<PaginatedResponse<UserResponse>>.Ok(
            await userService.GetAllAsync(query, ct)));
    }

    /// <summary>Get the currently authenticated user's own profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    public async Task<IActionResult> GetMe(CancellationToken ct)
        => Ok(ApiResponse<UserResponse>.Ok(await userService.GetByIdAsync(User.GetUserId(), ct)));

    /// <summary>Get user by ID including roles and permissions. Requires users:read.</summary>
    [HttpGet("{id:long}")]
    [Authorize(Policy = PolicyNames.UsersRead)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
        => Ok(ApiResponse<UserResponse>.Ok(await userService.GetByIdAsync(id, ct)));

    /// <summary>Update user details. Optionally change password. Requires users:update.</summary>
    [HttpPut("{id:long}")]
    [Authorize(Policy = PolicyNames.UsersUpdate)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        var v = await updateValidator.ValidateAsync(req, ct);
        if (!v.IsValid)
            return BadRequest(ApiResponse.Fail("Validation failed", v.Errors.Select(e => e.ErrorMessage).ToList()));

        return Ok(ApiResponse<UserResponse>.Ok(
            await userService.UpdateAsync(id, req, ct), "User updated successfully"));
    }

    /// <summary>Soft-delete a user. Requires users:delete.</summary>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = PolicyNames.UsersDelete)]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await userService.DeleteAsync(id, ct);
        return Ok(ApiResponse.Ok("User deleted successfully"));
    }
}
