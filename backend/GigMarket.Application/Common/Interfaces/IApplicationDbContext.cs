using GigMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<SellerProfile> SellerProfiles { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
