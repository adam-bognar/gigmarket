using GigMarket.Application.Features.Auth.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthUserDto> RegisterAsync(string firstname, string lastname, string email, string password, CancellationToken ct);
        Task<AuthUserDto> LoginAsync(string email, string password, bool isPersistent, CancellationToken ct);
        Task LogoutAsync(CancellationToken ct);
        Task<AuthUserDto> GetByIdAsync(Guid userId, CancellationToken ct);
    }
}
