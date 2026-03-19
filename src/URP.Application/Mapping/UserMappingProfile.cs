using AutoMapper;
using URP.Application.DTOs.Users;
using URP.Domain.Entities;

namespace URP.Application.Mapping;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(d => d.FullName,
                o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Roles,
                o => o.MapFrom(s => s.UserRoles.Select(ur => ur.Role)))
            .ForMember(d => d.Permissions,
                o => o.MapFrom(s =>
                    s.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => rp.Permission.Name)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList()));
    }
}
