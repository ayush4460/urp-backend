using URP.Application.DTOs.Permissions;

namespace URP.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionResponse>> GetAllAsync(string? group, CancellationToken ct = default);
    Task<PermissionResponse>              GetByIdAsync(int id, CancellationToken ct = default);
    Task<PermissionResponse>              CreateAsync(CreatePermissionRequest request, CancellationToken ct = default);
    Task                                  AssignToRoleAsync(AssignPermissionRequest request, CancellationToken ct = default);
    Task                                  RemoveFromRoleAsync(AssignPermissionRequest request, CancellationToken ct = default);
    Task<IEnumerable<PermissionResponse>> GetByRoleAsync(int roleId, CancellationToken ct = default);
}
