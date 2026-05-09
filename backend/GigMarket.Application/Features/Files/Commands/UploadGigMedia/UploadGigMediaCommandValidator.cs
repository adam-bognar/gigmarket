using FluentValidation;
using GigMarket.Application.Features.Files.Common;

namespace GigMarket.Application.Features.Files.Commands.UploadGigMedia;

public sealed class UploadGigMediaCommandValidator : AbstractValidator<UploadGigMediaCommand>
{
    public UploadGigMediaCommandValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
        RuleFor(x => x.FileStream).NotNull();
        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(HasSupportedExtension)
            .WithMessage("Only JPG, PNG, MP4 allowed.");

        RuleFor(x => x)
            .Custom((command, context) =>
            {
                if (command.FileLength <= 0)
                {
                    context.AddFailure(nameof(command.FileLength), "File must not be empty.");
                    return;
                }

                var ext = Path.GetExtension(command.FileName);
                if (FileUploadRules.IsVideoExtension(ext) && command.FileLength > FileUploadRules.MaxVideoSize)
                {
                    context.AddFailure(nameof(command.FileLength), "File exceeds 75MB limit.");
                }

                if (FileUploadRules.IsImageExtension(ext) && command.FileLength > FileUploadRules.MaxImageSize)
                {
                    context.AddFailure(nameof(command.FileLength), "File exceeds 5MB limit.");
                }
            });
    }

    private static bool HasSupportedExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        return FileUploadRules.IsImageExtension(ext) || FileUploadRules.IsVideoExtension(ext);
    }

    
}


