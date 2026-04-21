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
    DbSet<User> IApplicationDbContext.Users => Users;
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<SellerLanguage> SellerLanguages => Set<SellerLanguage>();
    public DbSet<SellerSkill> SellerSkills => Set<SellerSkill>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SellerEducation> SellerEducations => Set<SellerEducation>();
    public DbSet<SellerCertification> SellerCertifications => Set<SellerCertification>();
    public DbSet<SellerOccupation> SellerOccupations => Set<SellerOccupation>();
    
    public DbSet<Gig> Gigs => Set<Gig>();
    public DbSet<GigCategory> GigCategories => Set<GigCategory>();
    public DbSet<GigSubcategory> GigSubcategories => Set<GigSubcategory>();
    public DbSet<GigTag> GigTags => Set<GigTag>();
    public DbSet<GigPackage> GigPackages => Set<GigPackage>();
    public DbSet<GigRequirement> GigRequirements => Set<GigRequirement>();
    public DbSet<GigRequirementChoice> GigRequirementChoices => Set<GigRequirementChoice>();
    public DbSet<GigPhoto> GigPhotos => Set<GigPhoto>();
    public DbSet<GigVideo> GigVideos => Set<GigVideo>();
    public DbSet<GigReview> GigReviews => Set<GigReview>();
    
    public DbSet<Order> Orders => Set<Order>(); 
    public DbSet<OrderDelivery> OrderDeliveries => Set<OrderDelivery>();
    public DbSet<OrderRevisionRequest> OrderRevisionRequests => Set<OrderRevisionRequest>();
    public DbSet<OrderDeliveryAttachment> OrderDeliveryAttachments => Set<OrderDeliveryAttachment>();
    
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

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
        
        builder.Entity<Gig>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Status)
                .HasConversion<string>();

            entity.HasOne(g => g.SellerProfile)
                .WithMany(sp => sp.Gigs)
                .HasForeignKey(g => g.SellerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.Category)
                .WithMany(c => c.Gigs)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Subcategory)
                .WithMany(s => s.Gigs)
                .HasForeignKey(g => g.SubcategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(g => g.Tags)
                .WithOne(t => t.Gig)
                .HasForeignKey(t => t.GigId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Packages)
                .WithOne(p => p.Gig)
                .HasForeignKey(p => p.GigId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Requirements)
                .WithOne(r => r.Gig)
                .HasForeignKey(r => r.GigId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Photos)
                .WithOne(p => p.Gig)
                .HasForeignKey(p => p.GigId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.Video)
                .WithOne(v => v.Gig)
                .HasForeignKey<GigVideo>(v => v.GigId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<GigPackage>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Tier).HasConversion<string>();
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        builder.Entity<GigCategory>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasData(
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Graphics & Design" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Digital Marketing" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Writing & Translation" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Programming & Tech" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Video & Animation" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Music & Audio" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Business" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Finance" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "AI Services" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), Name = "Photography" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Data" },
                new GigCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Lifestyle" }
            );
        });

        builder.Entity<GigSubcategory>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(s => new { s.CategoryId, s.Name }).IsUnique();

            entity.HasOne(s => s.Category)
                .WithMany(c => c.Subcategories)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb001"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Logo Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb002"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Brand Style Guides" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb003"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Illustration" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb004"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "UI & UX Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb005"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Flyer & Brochure Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb006"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Social Media Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb007"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "T-Shirt & Merchandise" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb008"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Packaging Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb009"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "Book & Album Cover" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb010"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "3D Design" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb011"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "SEO" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb012"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Social Media Marketing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb013"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Content Marketing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb014"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Email Marketing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb015"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Paid Advertising (PPC)" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb016"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Influencer Marketing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb017"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "Video Marketing" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb021"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Article & Blog Writing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb022"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Copywriting" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb023"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Translation" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb024"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Proofreading & Editing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb025"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "CV & Resume Writing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb026"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Creative Writing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb027"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), Name = "Technical Writing" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb031"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Web Development" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb032"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Mobile Apps" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb033"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Game Development" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb034"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Cybersecurity & Data Protection" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb035"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "DevOps & Cloud" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb036"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Databases" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb037"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "QA & Testing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb038"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Desktop Applications" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb039"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), Name = "Chatbots & Automation" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb041"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Video Editing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb042"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Explainer Videos" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb043"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Animation" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb044"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Intro & Outro Videos" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb045"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Subtitles & Captions" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb046"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Visual Effects (VFX)" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb047"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), Name = "Slideshow & Promo Videos" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb051"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Voice Over" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb052"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Music Production" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb053"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Mixing & Mastering" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb054"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Songwriting & Lyrics" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb055"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Jingles & Intros" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb056"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Sound Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb057"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), Name = "Podcast Editing" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb061"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Virtual Assistant" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb062"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Business Plans" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb063"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Market Research" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb064"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Project Management" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb065"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Presentation Design" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb066"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "HR & Recruiting" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb067"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), Name = "Legal Consulting" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb071"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Accounting & Bookkeeping" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb072"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Financial Planning" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb073"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Tax Advice" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb074"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Investment Research" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb075"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8"), Name = "Crypto & Blockchain" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb081"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "AI Chatbots" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb082"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "AI Art & Image Generation" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb083"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "AI Content Writing" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb084"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "Machine Learning Models" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb085"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9"), Name = "Prompt Engineering" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb091"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), Name = "Photo Editing & Retouching" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb092"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), Name = "Product Photography" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb093"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), Name = "Portrait Photography" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb094"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), Name = "Real Estate Photography" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb101"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Data Entry" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb102"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Data Analysis" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb103"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Data Visualization" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb104"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Web Scraping" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb105"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11"), Name = "Data Science & ML" },

                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb111"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Online Tutoring" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb112"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Fitness & Nutrition" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb113"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Life Coaching" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb114"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Astrology & Readings" },
                new GigSubcategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb115"), CategoryId = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12"), Name = "Gaming & Coaching" }
            );
        });

        builder.Entity<GigRequirement>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Type).HasConversion<string>();

            entity.HasMany(r => r.Choices)
                .WithOne(c => c.GigRequirement)
                .HasForeignKey(c => c.GigRequirementId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<GigPhoto>(entity =>
        {
            entity.HasKey(p => p.Id);
        });

        builder.Entity<GigVideo>(entity =>
        {
            entity.HasKey(v => v.Id);
        });

        builder.Entity<GigReview>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Rating).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(2000).IsRequired();

            entity.HasOne(r => r.Gig)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GigId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.GigId, r.ReviewerUserId }).IsUnique();
        });
        
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
 
            entity.Property(o => o.Status)
                .HasConversion<string>();
 
            entity.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");
 
            entity.HasIndex(o => o.StripeSessionId)
                .IsUnique();
 
            entity.HasOne(o => o.Gig)
                .WithMany()
                .HasForeignKey(o => o.GigId)
                .OnDelete(DeleteBehavior.Restrict);
 
            entity.HasOne(o => o.Package)
                .WithMany()
                .HasForeignKey(o => o.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
 
            entity.HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.BuyerUserId)
                .OnDelete(DeleteBehavior.Restrict);
 
            entity.HasMany(o => o.Deliveries)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
 
            entity.HasMany(o => o.RevisionRequests)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        
        builder.Entity<OrderDelivery>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Message)
                .HasMaxLength(4000)
                .IsRequired();

            entity.HasMany(d => d.Attachments)
                .WithOne(a => a.OrderDelivery)
                .HasForeignKey(a => a.OrderDeliveryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderDeliveryAttachment>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.FileUrl)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(a => a.FileName)
                .HasMaxLength(255);

            entity.Property(a => a.ContentType)
                .HasMaxLength(255);

            entity.HasIndex(a => a.OrderDeliveryId);
        });
 
        builder.Entity<OrderRevisionRequest>(entity =>
        {
            entity.HasKey(r => r.Id);
 
            entity.Property(r => r.Message)
                .HasMaxLength(4000)
                .IsRequired();
        });
        
        builder.Entity<Conversation>(entity =>
        {
            entity.HasKey(c => c.Id);
 
            entity.HasIndex(c => new { c.BuyerUserId, c.SellerUserId, c.GigId }).IsUnique();
 
            entity.HasIndex(c => new { c.BuyerUserId,  c.LastMessageAtUtc });
            entity.HasIndex(c => new { c.SellerUserId, c.LastMessageAtUtc });
 
            entity.HasOne(c => c.Buyer)
                .WithMany()
                .HasForeignKey(c => c.BuyerUserId)
                .OnDelete(DeleteBehavior.Restrict);
 
            entity.HasOne(c => c.Seller)
                .WithMany()
                .HasForeignKey(c => c.SellerUserId)
                .OnDelete(DeleteBehavior.Restrict);
 
            entity.HasOne(c => c.Gig)
                .WithMany()
                .HasForeignKey(c => c.GigId)
                .OnDelete(DeleteBehavior.Cascade);
 
            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
 
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
 
            entity.HasIndex(m => new { m.ConversationId, m.SentAtUtc });
 
            entity.Property(m => m.Content)
                .HasMaxLength(4000)
                .IsRequired();
 
            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
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
