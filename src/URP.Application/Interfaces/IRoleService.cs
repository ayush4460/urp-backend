using URP.Application.DTOs.Roles;
using URP.Application.DTOs.Users;

namespace URP.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(CancellationToken ct = default);
    Task<RoleResponse>              GetByIdAsync(int id, CancellationToken ct = default);
    Task<RoleResponse>              CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<RoleResponse>              UpdateAsync(int id, CreateRoleRequest request, CancellationToken ct = default);
    Task                            DeleteAsync(int id, CancellationToken ct = default);
    Task                            AssignToUserAsync(AssignRoleRequest request, CancellationToken ct = default);
    Task                            RemoveFromUserAsync(RemoveRoleRequest request, CancellationToken ct = default);
    Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(int roleId, CancellationToken ct = default);
}
