using GigMarket.Application.Common.Interfaces;
using System.Security.Claims;

namespace GigMarket.Api.Services
{
    public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
    {
        public Guid? UserId
        {
            get
            {
                var raw = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(raw, out var id) ? id : null;
            }
        }
        public string? Email => accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
