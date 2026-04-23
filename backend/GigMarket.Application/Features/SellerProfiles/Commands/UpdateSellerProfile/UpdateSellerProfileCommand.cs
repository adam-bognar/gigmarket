using MediatR;

namespace GigMarket.Application.Features.SellerProfiles.Commands.UpdateSellerProfile
{
    public sealed record UpdateSellerProfileCommand(
        UpdateSellerProfileRequest Request) : IRequest;

    public sealed record UpdateSellerProfileRequest(
        string FirstName,
        string LastName,
        string ProfilePicUrl,
        string Description,
        List<Guid> LanguageIds,
        UpdateOccupationRequest Occupation,
        List<string> Skills,
        List<UpdateEducationRequest>? Educations,
        List<UpdateCertificationRequest>? Certifications,
        string? PersonalWebsite
    );

    public sealed record UpdateOccupationRequest(
        string OccupationName,
        int OccupationFromYear,
        int OccupationToYear);

    public sealed record UpdateEducationRequest(
        string Country,
        string InstitutionName,
        string Degree,
        string Major,
        int GraduationYear);

    public sealed record UpdateCertificationRequest(
        string Name,
        string IssuingOrganization,
        int Year);
}