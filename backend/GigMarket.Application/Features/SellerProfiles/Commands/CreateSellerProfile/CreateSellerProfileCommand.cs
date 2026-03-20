using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using GigMarket.Domain.Entities;

namespace GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile
{
    public sealed record CreateSellerProfileCommand(
        CreateSellerProfileRequest SellerProfileRequest) : IRequest<SellerProfileDto>;

    public sealed record CreateSellerProfileRequest(
        string FirstName,
        string LastName,
        string ProfilePicUrl,
        string Description,
        List<Guid> LanguageIds,
        OccupationRequest Occupation,
        List<string> Skills,
        List<EducationRequest>? Educations,
        List<CertificationRequest>? Certifications,
        string? PersonalWebsite
    );
    
    public sealed record OccupationRequest(
        string OccupationName,
        int OccupationFromYear,
        int OccupationToYear);

    public sealed record EducationRequest(
        string Country,
        string InstitutionName,
        string Degree,
        string Major,
        int GraduationYear);

    public sealed record CertificationRequest(
        string Name,
        string IssuingOrganization,
        int Year);
}
