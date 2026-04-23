namespace GigMarket.Application.Features.SellerProfiles.Models
{
    public sealed record SellerProfileDto(
        Guid Id,
        Guid UserId,
        DateTime CreatedAtUtc);

    public sealed record SellerProfileFullDto(
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string Description,
        string ProfileImageUrl,
        string? PersonalWebsite,
        SellerOccupationDto Occupation,
        List<SellerLanguageDto> Languages,
        List<string> Skills,
        List<SellerEducationDto> Educations,
        List<SellerCertificationDto> Certifications,
        DateTime CreatedAtUtc);

    public sealed record SellerOccupationDto(string Name, int FromYear, int ToYear);
    public sealed record SellerLanguageDto(Guid Id, string Name);
    public sealed record SellerEducationDto(string Country, string InstitutionName, string Degree, string Major, int GraduationYear);
    public sealed record SellerCertificationDto(string Name, string IssuingOrganization, int Year);
}