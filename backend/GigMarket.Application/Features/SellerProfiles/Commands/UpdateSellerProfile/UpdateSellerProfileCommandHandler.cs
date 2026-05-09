using GigMarket.Application.Common.Interfaces;
using MediatR;

namespace GigMarket.Application.Features.SellerProfiles.Commands.UpdateSellerProfile
{
    public sealed class UpdateSellerProfileCommandHandler(ISellerService sellerService)
        : IRequestHandler<UpdateSellerProfileCommand>
    {
        public Task Handle(UpdateSellerProfileCommand request, CancellationToken ct)
            => sellerService.UpdateSellerAsync(request.Request, ct);
    }
}