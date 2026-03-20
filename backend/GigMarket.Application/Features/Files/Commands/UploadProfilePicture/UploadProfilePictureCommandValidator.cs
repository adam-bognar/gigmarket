using FluentValidation;
using GigMarket.Application.Features.Files.Common;

namespace GigMarket.Application.Features.Files.Commands.UploadProfilePicture;

public sealed class UploadProfilePictureCommandValidator : AbstractValidator<UploadProfilePictureCommand>
{
    public UploadProfilePictureCommandValidator()
    {
        RuleFor(x => x.FileStream).NotNull();
        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(name => FileUploadRules.IsImageExtension(Path.GetExtension(name)))
            .WithMessage("Only JPG/PNG allowed.");

        RuleFor(x => x.FileLength)
            .GreaterThan(0)
            .LessThanOrEqualTo(FileUploadRules.MaxImageSize)
            .WithMessage("File exceeds 5MB limit.");
    }
}

