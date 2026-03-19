using URP.Application.Common;
using URP.Application.DTOs.Auth;
using URP.Application.DTOs.Users;

namespace URP.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse>             RegisterAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<LoginResponse>            LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UserResponse>             GetByIdAsync(long id, CancellationToken ct = default);
    Task<PaginatedResponse<UserResponse>> GetAllAsync(PaginationQuery query, CancellationToken ct = default);
    Task<UserResponse>             UpdateAsync(long id, UpdateUserRequest request, CancellationToken ct = default);
    Task                           DeleteAsync(long id, CancellationToken ct = default);
}
