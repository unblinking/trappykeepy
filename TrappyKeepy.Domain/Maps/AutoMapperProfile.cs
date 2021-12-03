using AutoMapper;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Maps
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<GroupDto, Group>();
            CreateMap<Keeper, KeeperDto>();
            CreateMap<KeeperDto, Keeper>();
            CreateMap<Membership, MembershipDto>();
            CreateMap<MembershipDto, Membership>();
            CreateMap<Permit, PermitDto>();
            CreateMap<PermitDto, Permit>();
            CreateMap<User, UserDto>()
                // Always ignore User.Password when mapping to UserDTO
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<UserDto, User>();
        }
    }
}
