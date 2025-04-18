using AutoMapper;
using BloggingSystem.Domain.Entities;
using BloggingSystem.Shared.DTOs;

namespace BloggingSystem.Application.Mappings;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserProfile.DisplayName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.UserProfile.AvatarUrl))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserProfile.Bio))
            .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.UserProfile.Website))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.UserProfile.Location))
            .ReverseMap();
        CreateMap<User, UserSummaryDto>();
        CreateMap<User, AuthorDto>();
        
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
        CreateMap<Post, PostSummaryDto>().ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.PostCategories.Select(pc => pc.Category)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag)));
        CreateMap<Post, PostDto>().ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src.PostCategories.Select(pc => pc.Category)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag)))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

        // Category -> CategoryDto
        CreateMap<Category, CategoryDto>();
        // Tag -> TagDto
        CreateMap<Tag, TagDto>();
        
        // Revision -> RevisionDto
        CreateMap<Revision, RevisionDto>()
            .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.Post.Title))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
        
        // Like -> LikeDto
        CreateMap<Like, LikeDto>();
        
    }
}