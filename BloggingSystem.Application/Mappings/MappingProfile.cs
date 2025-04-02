using AutoMapper;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserSummaryDto>();
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<Role, RoleSummaryDto>();
        
        CreateMap<Permission, PermissionDto>().ReverseMap();
        
        // Comment -> CommentDto
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Replies, opt => opt.Ignore()); // Replies are handled specially

        // User -> UserSummaryDto
        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => 
                src.UserProfile != null ? src.UserProfile.DisplayName : src.Username))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => 
                src.UserProfile != null ? src.UserProfile.AvatarUrl : null));

        // Post -> PostSummaryDto
        CreateMap<Post, PostSummaryDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
    }
}