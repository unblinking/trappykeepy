using AutoMapper;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Domain.Maps
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Group, IGroupDto>();
            CreateMap<IGroupDto, Group>();

            CreateMap<Keeper, IKeeperDto>();
            CreateMap<IKeeperDto, Keeper>();

            CreateMap<Membership, IMembershipDto>();
            CreateMap<IMembershipDto, Membership>();

            CreateMap<Permit, IPermitDto>();
            CreateMap<IPermitDto, Permit>();

            CreateMap<User, IUserDto>()
                // Always ignore User.Password when mapping to UserDTO
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<IUserDto, User>();
            CreateMap<IUserSessionDto, User>();
        }
    }
}
