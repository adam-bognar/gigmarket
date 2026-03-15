using GigMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<SellerProfile> SellerProfiles { get; }
        DbSet<Language> Languages { get; }
        DbSet<SellerLanguage> SellerLanguages { get; }
        DbSet<SellerSkill> SellerSkills { get; }
        DbSet<Skill> Skills { get; }
        DbSet<SellerEducation> SellerEducations { get; }
        DbSet<SellerCertification> SellerCertifications { get; }
        DbSet<SellerOccupation> SellerOccupations { get; }
        
        DbSet<Gig> Gigs { get; }
        DbSet<GigTag> GigTags { get; }
        DbSet<GigPackage> GigPackages { get; }
        DbSet<GigRequirement> GigRequirements { get; }
        DbSet<GigRequirementChoice> GigRequirementChoices { get; }
        DbSet<GigPhoto> GigPhotos { get; }
        DbSet<GigVideo> GigVideos { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}