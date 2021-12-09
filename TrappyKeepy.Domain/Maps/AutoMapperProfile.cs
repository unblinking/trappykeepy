using AutoMapper;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Maps
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<Group, GroupComplexDto>()
                .ForMember(dest => dest.Memberships, opt => opt.Condition(source => source.Memberships.Count() > 0))
                .ForMember(dest => dest.Permits, opt => opt.Condition(source => source.Permits.Count() > 0));
            CreateMap<GroupDto, Group>();

            CreateMap<Keeper, KeeperDto>();
            CreateMap<KeeperDto, Keeper>();

            CreateMap<Membership, MembershipDto>();
            CreateMap<MembershipDto, Membership>();

            CreateMap<Permit, PermitDto>();
            CreateMap<PermitDto, Permit>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Always ignore User.Password when mapping to UserDTO
            CreateMap<User, UserComplexDto>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Always ignore User.Password when mapping to UserComplexDTO
                .ForMember(dest => dest.Keepers, opt => opt.Condition(source => source.Keepers.Count() > 0))
                .ForMember(dest => dest.Memberships, opt => opt.Condition(source => source.Memberships.Count() > 0))
                .ForMember(dest => dest.Permits, opt => opt.Condition(source => source.Permits.Count() > 0));
            CreateMap<UserDto, User>();
            CreateMap<UserSessionDto, User>();
        }
    }
}
