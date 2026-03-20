using FluentValidation;

namespace GigMarket.Application.Features.Files.Commands.DeleteFile;

public sealed class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {
        RuleFor(x => x.BlobPath).NotEmpty();
    }
}

