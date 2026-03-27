using AutoMapper;
using GigMarket.Application.Features.Categories.Models;
using GigMarket.Application.Features.Reviews.Models;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;

namespace GigMarket.Application.Common.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GigCategory, GigCategoryDto>();
        CreateMap<GigSubcategory, GigSubcategoryDto>();
        
        CreateMap<GigPackage, GigPackageDto>()
            .ForCtorParam(nameof(GigPackageDto.Tier), opt => opt.MapFrom(src => src.Tier.ToString()));

        CreateMap<Gig, GigDto>()
            .ForCtorParam(nameof(GigDto.Category),    opt => opt.MapFrom(src => src.Category.Name))
            .ForCtorParam(nameof(GigDto.Subcategory), opt => opt.MapFrom(src => src.Subcategory.Name))
            .ForCtorParam(nameof(GigDto.Status),      opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam(nameof(GigDto.Tags),        opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
            .ForCtorParam(nameof(GigDto.Packages),    opt => opt.MapFrom(src => src.Packages))
            .ForCtorParam(nameof(GigDto.Photos),      opt => opt.MapFrom(src => src));

        CreateMap<Gig, GigPhotosDto>()
            .ForCtorParam(nameof(GigPhotosDto.PrimaryPhotoUrl),
                opt => opt.MapFrom(src => src.Photos.First(p => p.IsPrimary).Url))
            .ForCtorParam(nameof(GigPhotosDto.AdditionalPhotoUrls),
                opt => opt.MapFrom(src => src.Photos
                    .Where(p => !p.IsPrimary)
                    .OrderBy(p => p.SortOrder)
                    .Select(p => p.Url)
                    .ToList()));

        CreateMap<SellerProfile, SellerProfileDto>();

        CreateMap<GigReview, ReviewDto>()
            .ForCtorParam(nameof(ReviewDto.ReviewerUsername), opt => opt.MapFrom(src => src.Reviewer.CustomUsername));
    }
}

