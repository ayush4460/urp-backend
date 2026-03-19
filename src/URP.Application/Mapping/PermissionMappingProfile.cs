using AutoMapper;
using URP.Application.DTOs.Permissions;
using URP.Domain.Entities;

namespace URP.Application.Mapping;

public sealed class PermissionMappingProfile : Profile
{
    public PermissionMappingProfile()
    {
        CreateMap<Permission, PermissionResponse>();
    }
}
