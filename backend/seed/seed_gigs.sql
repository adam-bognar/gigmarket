SET NOCOUNT ON;

PRINT 'Seeding gigs...';

IF NOT EXISTS (SELECT 1 FROM Gigs WHERE Id = '40000000-0000-0000-0000-000000000001')
INSERT INTO Gigs (Id, SellerProfileId, CategoryId, SubcategoryId, Title, Description, Status, CreatedAtUtc)
VALUES
('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001',
 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb031',
 'I will build a modern full-stack web application',
 'I will build a responsive full-stack web application with Angular, ASP.NET Core and SQL Server. This is ideal for dashboards, booking systems, admin panels, marketplaces and startup MVPs.',
 'Active', DATEADD(day, -30, SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM Gigs WHERE Id = '40000000-0000-0000-0000-000000000002')
INSERT INTO Gigs (Id, SellerProfileId, CategoryId, SubcategoryId, Title, Description, Status, CreatedAtUtc)
VALUES
('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001',
 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb035',
 'I will deploy your .NET app to Azure',
 'I will prepare your ASP.NET Core backend for cloud deployment, configure environment variables, storage settings and basic production-ready hosting structure.',
 'Active', DATEADD(day, -18, SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM Gigs WHERE Id = '40000000-0000-0000-0000-000000000003')
INSERT INTO Gigs (Id, SellerProfileId, CategoryId, SubcategoryId, Title, Description, Status, CreatedAtUtc)
VALUES
('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000002',
 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb001',
 'I will design a clean modern logo for your brand',
 'I will create a modern and memorable logo concept for your brand, startup, webshop or personal project. The package can include color palette and basic usage notes.',
 'Active', DATEADD(day, -26, SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM Gigs WHERE Id = '40000000-0000-0000-0000-000000000004')
INSERT INTO Gigs (Id, SellerProfileId, CategoryId, SubcategoryId, Title, Description, Status, CreatedAtUtc)
VALUES
('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000002',
 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb004',
 'I will design a beautiful landing page UI in Figma',
 'I will design a polished landing page UI for your product, app or service. The design will focus on clear structure, conversion and a clean visual style.',
 'Active', DATEADD(day, -14, SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM Gigs WHERE Id = '40000000-0000-0000-0000-000000000005')
INSERT INTO Gigs (Id, SellerProfileId, CategoryId, SubcategoryId, Title, Description, Status, CreatedAtUtc)
VALUES
('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000003',
 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbb041',
 'I will edit short-form videos for TikTok, Reels and Shorts',
 'I will edit engaging short-form videos with captions, cuts, music, transitions and pacing optimized for social media platforms.',
 'Active', DATEADD(day, -10, SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM GigPackages WHERE Id = '41000000-0000-0000-0000-000000000001')
INSERT INTO GigPackages (Id, GigId, Tier, Name, Description, DeliveryDays, Revisions, Price)
VALUES
('41000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000001','Basic','Landing page','One responsive landing page with clean structure.',3,1,80),
('41000000-0000-0000-0000-000000000002','40000000-0000-0000-0000-000000000001','Standard','Full frontend','Angular frontend with multiple pages and reusable components.',7,2,220),
('41000000-0000-0000-0000-000000000003','40000000-0000-0000-0000-000000000001','Premium','Full-stack MVP','Frontend, backend API, SQL database and basic deployment preparation.',14,3,650),

('41000000-0000-0000-0000-000000000004','40000000-0000-0000-0000-000000000002','Basic','Deployment check','I review your project and prepare deployment notes.',2,1,60),
('41000000-0000-0000-0000-000000000005','40000000-0000-0000-0000-000000000002','Standard','Azure deployment','I deploy your app and configure environment variables.',4,2,180),
('41000000-0000-0000-0000-000000000006','40000000-0000-0000-0000-000000000002','Premium','Production setup','Deployment, storage config, connection strings and basic monitoring notes.',7,3,350),

('41000000-0000-0000-0000-000000000007','40000000-0000-0000-0000-000000000003','Basic','One logo concept','One simple logo concept with PNG export.',3,1,45),
('41000000-0000-0000-0000-000000000008','40000000-0000-0000-0000-000000000003','Standard','Three concepts','Three logo concepts with source file.',5,2,120),
('41000000-0000-0000-0000-000000000009','40000000-0000-0000-0000-000000000003','Premium','Brand starter kit','Logo, colors, typography and small brand guide.',8,3,260),

('41000000-0000-0000-0000-000000000010','40000000-0000-0000-0000-000000000004','Basic','Hero section','Hero section and basic visual direction.',2,1,50),
('41000000-0000-0000-0000-000000000011','40000000-0000-0000-0000-000000000004','Standard','Landing page UI','Full landing page UI in Figma.',5,2,160),
('41000000-0000-0000-0000-000000000012','40000000-0000-0000-0000-000000000004','Premium','Landing page + mobile','Desktop and mobile landing page UI with components.',7,3,300),

('41000000-0000-0000-0000-000000000013','40000000-0000-0000-0000-000000000005','Basic','One short video','One edited video up to 30 seconds.',2,1,35),
('41000000-0000-0000-0000-000000000014','40000000-0000-0000-0000-000000000005','Standard','Three short videos','Three edited short-form videos with captions.',4,2,90),
('41000000-0000-0000-0000-000000000015','40000000-0000-0000-0000-000000000005','Premium','Weekly content pack','Seven short-form videos with captions and platform-ready exports.',7,3,220);

IF NOT EXISTS (SELECT 1 FROM GigTags WHERE GigId = '40000000-0000-0000-0000-000000000001')
INSERT INTO GigTags (Id, GigId, Name)
VALUES
(NEWID(),'40000000-0000-0000-0000-000000000001','angular'),
(NEWID(),'40000000-0000-0000-0000-000000000001','asp.net'),
(NEWID(),'40000000-0000-0000-0000-000000000001','fullstack'),
(NEWID(),'40000000-0000-0000-0000-000000000001','web app'),

(NEWID(),'40000000-0000-0000-0000-000000000002','azure'),
(NEWID(),'40000000-0000-0000-0000-000000000002','deployment'),
(NEWID(),'40000000-0000-0000-0000-000000000002','cloud'),

(NEWID(),'40000000-0000-0000-0000-000000000003','logo'),
(NEWID(),'40000000-0000-0000-0000-000000000003','branding'),
(NEWID(),'40000000-0000-0000-0000-000000000003','startup'),

(NEWID(),'40000000-0000-0000-0000-000000000004','figma'),
(NEWID(),'40000000-0000-0000-0000-000000000004','ui design'),
(NEWID(),'40000000-0000-0000-0000-000000000004','landing page'),

(NEWID(),'40000000-0000-0000-0000-000000000005','video editing'),
(NEWID(),'40000000-0000-0000-0000-000000000005','tiktok'),
(NEWID(),'40000000-0000-0000-0000-000000000005','reels');

IF NOT EXISTS (SELECT 1 FROM GigPhotos WHERE GigId = '40000000-0000-0000-0000-000000000001')
INSERT INTO GigPhotos (Id, GigId, Url, IsPrimary, SortOrder)
VALUES
(NEWID(),'40000000-0000-0000-0000-000000000001','gigs/demo/fullstack-primary.jpg',1,0),
(NEWID(),'40000000-0000-0000-0000-000000000001','gigs/demo/fullstack-dashboard.jpg',0,1),
(NEWID(),'40000000-0000-0000-0000-000000000002','gigs/demo/azure-primary.jpg',1,0),
(NEWID(),'40000000-0000-0000-0000-000000000003','gigs/demo/logo-primary.jpg',1,0),
(NEWID(),'40000000-0000-0000-0000-000000000003','gigs/demo/logo-secondary.jpg',0,1),
(NEWID(),'40000000-0000-0000-0000-000000000004','gigs/demo/landing-ui-primary.jpg',1,0),
(NEWID(),'40000000-0000-0000-0000-000000000005','gigs/demo/video-edit-primary.jpg',1,0);

IF NOT EXISTS (SELECT 1 FROM GigVideos WHERE GigId = '40000000-0000-0000-0000-000000000005')
INSERT INTO GigVideos (Id, GigId, Url)
VALUES
(NEWID(),'40000000-0000-0000-0000-000000000005','gigs/demo/video-edit-preview.mp4');

IF NOT EXISTS (SELECT 1 FROM GigRequirements WHERE GigId = '40000000-0000-0000-0000-000000000001')
BEGIN
    DECLARE @Req1 uniqueidentifier = '42000000-0000-0000-0000-000000000001';
    DECLARE @Req2 uniqueidentifier = '42000000-0000-0000-0000-000000000002';
    DECLARE @Req3 uniqueidentifier = '42000000-0000-0000-0000-000000000003';

    INSERT INTO GigRequirements (Id, GigId, Type, Question, IsRequired, SortOrder)
    VALUES
    (@Req1,'40000000-0000-0000-0000-000000000001','FreeText','Describe the app idea and the main pages you need.',1,0),
    (@Req2,'40000000-0000-0000-0000-000000000001','MultipleChoice','Do you already have a design?',1,1),
    (@Req3,'40000000-0000-0000-0000-000000000003','FileUpload','Upload any logo inspiration, sketches or brand references.',0,0);

    INSERT INTO GigRequirementChoices (Id, GigRequirementId, Value)
    VALUES
    (NEWID(), @Req2, 'Yes, I have a Figma design'),
    (NEWID(), @Req2, 'I have rough sketches'),
    (NEWID(), @Req2, 'No, I need design help too');
END

PRINT 'Gig seed done.';