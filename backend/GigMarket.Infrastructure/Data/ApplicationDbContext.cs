using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<SellerLanguage> SellerLanguages => Set<SellerLanguage>();
    public DbSet<SellerSkill> SellerSkills => Set<SellerSkill>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SellerEducation> SellerEducations => Set<SellerEducation>();
    public DbSet<SellerCertification> SellerCertifications => Set<SellerCertification>();
    public DbSet<SellerOccupation> SellerOccupations => Set<SellerOccupation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SellerProfile>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.UserId).IsUnique();

            entity.HasOne(x => x.User)
                .WithOne(u => u.SellerProfile)
                .HasForeignKey<SellerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(sp => sp.Certifications)
                .WithOne(c => c.SellerProfile)
                .HasForeignKey(c => c.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(sp => sp.Educations)
                .WithOne(e => e.SellerProfile)
                .HasForeignKey(e => e.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(sp => sp.Occupation)
                .WithOne(o => o.SellerProfile)
                .HasForeignKey<SellerOccupation>(o => o.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SellerLanguage>(entity =>
        {
            entity.HasKey(sl => new { SellerId = sl.SellerProfileId, sl.LanguageId });
            
            entity.HasOne(sl => sl.SellerProfile)
                .WithMany(sp => sp.Languages)
                .HasForeignKey(sl => sl.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(sl => sl.Language)
                .WithMany()
                .HasForeignKey(sl => sl.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<SellerSkill>(entity =>
        {
            entity.HasKey(ss => new { SellerId = ss.SellerProfileId, ss.SkillId });
            
            entity.HasOne(ss => ss.SellerProfile)
                .WithMany(sp => sp.Skills)
                .HasForeignKey(ss => ss.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(ss => ss.Skill)
                .WithMany()
                .HasForeignKey(ss => ss.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<Language>().HasData(
            new Language { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "English" },
            new Language { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Spanish" },
            new Language { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "French" },
            new Language { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "German" },
            new Language { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Chinese" }
        );
    }
}
