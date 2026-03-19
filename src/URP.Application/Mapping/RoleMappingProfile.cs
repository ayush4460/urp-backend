using AutoMapper;
using URP.Application.DTOs.Roles;
using URP.Domain.Entities;

namespace URP.Application.Mapping;

public sealed class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleResponse>()
            .ForMember(d => d.Permissions,
                o => o.MapFrom(s => s.RolePermissions.Select(rp => rp.Permission)));
    }
}
