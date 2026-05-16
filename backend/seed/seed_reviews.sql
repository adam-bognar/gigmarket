SET NOCOUNT ON;

PRINT 'Seeding reviews...';

IF NOT EXISTS (SELECT 1 FROM GigReviews WHERE Id = '60000000-0000-0000-0000-000000000001')
INSERT INTO GigReviews (Id, GigId, ReviewerUserId, Rating, Description, CreatedAtUtc)
VALUES
('60000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000002',5,'Mia understood the brand direction immediately. The logo concepts were clean and easy to use.',DATEADD(day,-14,SYSUTCDATETIME())),

('60000000-0000-0000-0000-000000000002','40000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000002',5,'Alex was very clear with communication and helped me understand what features should go into the MVP.',DATEADD(day,-8,SYSUTCDATETIME())),

('60000000-0000-0000-0000-000000000003','40000000-0000-0000-0000-000000000005','10000000-0000-0000-0000-000000000001',4,'The videos were energetic and well paced. I only needed a tiny change in one caption.',DATEADD(day,-1,SYSUTCDATETIME()));

PRINT 'Review seed done.';