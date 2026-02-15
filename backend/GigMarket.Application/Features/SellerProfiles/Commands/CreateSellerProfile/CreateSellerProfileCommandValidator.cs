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
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Bio).MaximumLength(1000);
        }
    }
}
