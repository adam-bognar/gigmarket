using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile
{
    public sealed class CreateSellerProfileCommandValidator : AbstractValidator<CreateSellerProfileCommand>
    {
        public CreateSellerProfileCommandValidator()
        {
            RuleFor(x => x.SellerProfileRequest).NotNull().SetValidator(new CreateSellerProfileRequestValidator());
        }
    }
    
    public sealed class CreateSellerProfileRequestValidator : AbstractValidator<CreateSellerProfileRequest>
    {
        public CreateSellerProfileRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ProfilePicUrl).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MinimumLength(50).MaximumLength(1000);

            RuleFor(x => x.LanguageIds).NotEmpty().WithMessage("At least one language is required.");
            RuleForEach(x => x.LanguageIds).NotEmpty().WithMessage("Language ID cannot be empty.");

            RuleFor(x => x.Occupation).NotNull().SetValidator(new OccupationRequestValidator());

            RuleFor(x => x.Skills).NotEmpty().WithMessage("At least one skill is required.");
            RuleForEach(x => x.Skills).NotEmpty().MaximumLength(50);

            RuleForEach(x => x.Educations).SetValidator(new EducationRequestValidator());
            RuleForEach(x => x.Certifications).SetValidator(new CertificationRequestValidator());

            RuleFor(x => x.PersonalWebsite).MaximumLength(200).Must(uri =>
                    Uri.TryCreate(uri, UriKind.Absolute, out var result) &&
                    (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
                .When(x => !string.IsNullOrWhiteSpace(x.PersonalWebsite))
                .WithMessage("Personal website must be a valid URL.");
        }
    }
    
    public sealed class OccupationRequestValidator : AbstractValidator<OccupationRequest>
    {
        public OccupationRequestValidator()
        {
            var currentYear = DateTime.UtcNow.Year;

            RuleFor(x => x.OccupationName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.OccupationFromYear).InclusiveBetween(1950, currentYear);
            RuleFor(x => x.OccupationToYear).InclusiveBetween(1950, currentYear);
            RuleFor(x => x).Must(x => x.OccupationToYear >= x.OccupationFromYear)
                .WithName("OccupationToYear")
                .WithMessage("End year must be greater than or equal to start year.");
        }
    }
    
    public sealed class EducationRequestValidator : AbstractValidator<EducationRequest>
    {
        public EducationRequestValidator()
        {
            var currentYear = DateTime.UtcNow.Year;

            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.InstitutionName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Degree).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Major).NotEmpty().MaximumLength(100);
            RuleFor(x => x.GraduationYear).InclusiveBetween(1950, currentYear + 10);
        }
    }
    
    public sealed class CertificationRequestValidator : AbstractValidator<CertificationRequest>
    {
        public CertificationRequestValidator()
        {
            var currentYear = DateTime.UtcNow.Year;

            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.IssuingOrganization).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Year).InclusiveBetween(1950, currentYear);
        }
    }
}
