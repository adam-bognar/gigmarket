using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Commands.UpdateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Infrastructure.Service;

public sealed class SellerService(ICurrentUserService currentUser, IApplicationDbContext db) : ISellerService
{
    public async Task<SellerProfileDto> CreateSellerAsync(CreateSellerProfileRequest request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!;

        var exists = await db.SellerProfiles.AnyAsync(x => x.UserId == userId, ct);
        if (exists) throw new BadRequestException("Seller profile already exists.");

        var languages = await db.Languages
            .Where(l => request.LanguageIds.Contains(l.Id))
            .ToListAsync(ct);

        if (languages.Count != request.LanguageIds.Count)
            throw new BadRequestException("One or more language IDs are invalid.");

        var skillNames = request.Skills.Select(s => s.ToLower()).ToList();
        var existingSkills = await db.Skills
            .Where(s => skillNames.Contains(s.Name.ToLower()))
            .ToListAsync(ct);

        var existingSkillNames = existingSkills.Select(s => s.Name.ToLower()).ToHashSet();
        var newSkills = skillNames
            .Where(name => !existingSkillNames.Contains(name))
            .Select(name => new Skill { Id = Guid.NewGuid(), Name = name })
            .ToList();

        db.Skills.AddRange(newSkills);
        
        var allSkills = existingSkills.Concat(newSkills).ToList();

        var seller = new SellerProfile
        {
            Id = Guid.NewGuid(),
            UserId = (Guid)userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Description = request.Description,
            ProfileImageUrl = request.ProfilePicUrl,
            PersonalWebsite = request.PersonalWebsite,
            Occupation = new SellerOccupation()
            {
                Id = Guid.NewGuid(),
                Name = request.Occupation.OccupationName,
                FromYear = request.Occupation.OccupationFromYear,
                ToYear = request.Occupation.OccupationToYear
            },
            Languages = languages.Select(l => new SellerLanguage
            {
                LanguageId = l.Id
            }).ToList(),
            Skills = allSkills.Select(s => new SellerSkill
            {
                SkillId = s.Id
            }).ToList(),
            Educations = request.Educations?.Select(e => new SellerEducation
            {
                Id = Guid.NewGuid(),
                Country = e.Country,
                InstitutionName = e.InstitutionName,
                Degree = e.Degree,
                Major = e.Major,
                GraduationYear = e.GraduationYear
            }).ToList() ?? new List<SellerEducation>(),
            Certifications = request.Certifications?.Select(c => new SellerCertification
            {
                Id = Guid.NewGuid(),
                Name = c.Name,
                IssuingOrganization = c.IssuingOrganization,
                Year = c.Year
            }).ToList() ?? new List<SellerCertification>()
        };

        db.SellerProfiles.Add(seller);
        await db.SaveChangesAsync(ct);

        return new SellerProfileDto(seller.Id, seller.UserId, seller.CreatedAtUtc);
    }
    
    public async Task UpdateSellerAsync(UpdateSellerProfileRequest request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
            throw new UnauthorizedException("Not authenticated.");
 
        var userId = currentUser.UserId!;
 
        var seller = await db.SellerProfiles
            .Include(s => s.Occupation)
            .Include(s => s.Languages)
            .Include(s => s.Skills)
            .Include(s => s.Educations)
            .Include(s => s.Certifications)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
 
        if (seller is null) throw new NotFoundException("Seller profile not found.");
 
        db.SellerOccupations.Remove(seller.Occupation);
        db.SellerLanguages.RemoveRange(seller.Languages);
        db.SellerSkills.RemoveRange(seller.Skills);
        db.SellerEducations.RemoveRange(seller.Educations);
        db.SellerCertifications.RemoveRange(seller.Certifications);
 
        await db.SaveChangesAsync(ct);
 
        seller.FirstName = request.FirstName;
        seller.LastName = request.LastName;
        seller.Description = request.Description;
        seller.ProfileImageUrl = request.ProfilePicUrl;
        seller.PersonalWebsite = request.PersonalWebsite;
 
        var languages = await db.Languages
            .Where(l => request.LanguageIds.Contains(l.Id))
            .ToListAsync(ct);
 
        if (languages.Count != request.LanguageIds.Count)
            throw new BadRequestException("One or more language IDs are invalid.");
 
        var skillNames = request.Skills.Select(s => s.ToLower()).ToList();
        var existingSkills = await db.Skills
            .Where(s => skillNames.Contains(s.Name.ToLower()))
            .ToListAsync(ct);
 
        var existingSkillNames = existingSkills.Select(s => s.Name.ToLower()).ToHashSet();
        var newSkills = skillNames
            .Where(name => !existingSkillNames.Contains(name))
            .Select(name => new Skill { Id = Guid.NewGuid(), Name = name })
            .ToList();
 
        db.Skills.AddRange(newSkills);
 
        var allSkills = existingSkills.Concat(newSkills).ToList();
 
        seller.Occupation = new SellerOccupation
        {
            Id = Guid.NewGuid(),
            Name = request.Occupation.OccupationName,
            FromYear = request.Occupation.OccupationFromYear,
            ToYear = request.Occupation.OccupationToYear
        };
 
        seller.Languages = languages.Select(l => new SellerLanguage { LanguageId = l.Id }).ToList();
        seller.Skills = allSkills.Select(s => new SellerSkill { SkillId = s.Id }).ToList();
        seller.Educations = request.Educations?.Select(e => new SellerEducation
        {
            Id = Guid.NewGuid(),
            Country = e.Country,
            InstitutionName = e.InstitutionName,
            Degree = e.Degree,
            Major = e.Major,
            GraduationYear = e.GraduationYear
        }).ToList() ?? [];
 
        seller.Certifications = request.Certifications?.Select(c => new SellerCertification
        {
            Id = Guid.NewGuid(),
            Name = c.Name,
            IssuingOrganization = c.IssuingOrganization,
            Year = c.Year
        }).ToList() ?? [];
 
        await db.SaveChangesAsync(ct);
    }
}