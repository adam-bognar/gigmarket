using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile
{
    public sealed class CreateSellerProfileCommandHandler(
        ISellerService sellerService) : IRequestHandler<CreateSellerProfileCommand, SellerProfileDto>
    {
        public Task<SellerProfileDto> Handle(CreateSellerProfileCommand request, CancellationToken ct)
        => sellerService.CreateSellerAsync(request.SellerProfileRequest, ct);
    }
}