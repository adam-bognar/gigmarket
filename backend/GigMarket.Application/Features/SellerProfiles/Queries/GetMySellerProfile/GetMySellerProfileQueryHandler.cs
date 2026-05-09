using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetMySellerProfile
{
    public sealed class GetMySellerProfileQueryHandler(ICurrentUserService currentUser, IApplicationDbContext db, IBlobStorageService blobStorageService)
        : IRequestHandler<GetMySellerProfileQuery, SellerProfileFullDto>
    {
        public async Task<SellerProfileFullDto> Handle(GetMySellerProfileQuery request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
                throw new UnauthorizedException("Not authenticated.");

            var userId = currentUser.UserId!;

            var entity = await db.SellerProfiles
                .AsNoTracking()
                .Include(s => s.Occupation)
                .Include(s => s.Languages).ThenInclude(l => l.Language)
                .Include(s => s.Skills).ThenInclude(sk => sk.Skill)
                .Include(s => s.Educations)
                .Include(s => s.Certifications)
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (entity is null) throw new NotFoundException("Seller profile not found.");
            
            var imageUrl = await ResolveUrlAsync(entity.ProfileImageUrl, ct);

            return new SellerProfileFullDto(
                entity.Id,
                entity.UserId,
                entity.FirstName,
                entity.LastName,
                entity.Description,
                imageUrl,
                entity.PersonalWebsite,
                new SellerOccupationDto(
                    entity.Occupation.Name,
                    entity.Occupation.FromYear,
                    entity.Occupation.ToYear),
                entity.Languages.Select(l => new SellerLanguageDto(l.LanguageId, l.Language.Name)).ToList(),
                entity.Skills.Select(s => s.Skill.Name).ToList(),
                entity.Educations.Select(e => new SellerEducationDto(
                    e.Country, e.InstitutionName, e.Degree, e.Major, e.GraduationYear)).ToList(),
                entity.Certifications.Select(c => new SellerCertificationDto(
                    c.Name, c.IssuingOrganization, c.Year)).ToList(),
                entity.CreatedAtUtc);
        }
        
        private async Task<string> ResolveUrlAsync(string? blobPath, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(blobPath)) return string.Empty;
            if (blobPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                blobPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return blobPath;
            }

            return await blobStorageService.GetDownloadUrlAsync(blobPath, ct);
        }
    }
}