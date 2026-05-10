import { TestBed } from '@angular/core/testing';
import { GigDraftService } from './gig-draft.service';
import { GigDetailDto } from '../models/gig.model';

describe('GigDraftService', () => {
  let service: GigDraftService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GigDraftService);
  });

  it('should store overview draft', () => {
    const overview = {
      title: 'I will build your website',
      categoryId: 'cat-1',
      subcategoryId: 'sub-1',
      categoryName: 'Programming',
      subcategoryName: 'Web development',
      tags: ['angular', 'dotnet'],
      description: 'A detailed description for the gig.',
    };

    service.setOverview(overview);

    expect(service.overview()).toEqual(overview);
  });

  it('should store pricing draft', () => {
    const pricing = {
      packages: [
        {
          tier: 'Basic' as const,
          name: 'Basic',
          description: 'Basic package description',
          deliveryDays: 3,
          revisions: 1,
          price: 20,
        },
      ],
    };

    service.setPricing(pricing);

    expect(service.pricing()).toEqual(pricing);
  });

  it('should clear all draft data', () => {
    service.editingGigId.set('gig-1');
    service.setOverview({
      title: 'Test',
      categoryId: 'cat-1',
      subcategoryId: 'sub-1',
      categoryName: 'Category',
      subcategoryName: 'Subcategory',
      tags: ['tag'],
      description: 'Description',
    });

    service.clearDraft();

    expect(service.editingGigId()).toBeNull();
    expect(service.isEditMode()).toBe(false);
    expect(service.overview()).toBeNull();
    expect(service.pricing()).toBeNull();
    expect(service.requirements()).toBeNull();
    expect(service.gallery()).toBeNull();
  });

  it('should load draft data from existing gig for edit mode', () => {
    const gig: GigDetailDto = {
      id: 'gig-1',
      title: 'Existing gig',
      description: 'Existing description',
      status: 'Active',
      createdAtUtc: '2026-01-01T00:00:00Z',
      categoryId: 'cat-1',
      categoryName: 'Programming',
      subcategoryId: 'sub-1',
      subcategoryName: 'Web development',
      sellerProfileId: 'seller-1',
      sellerFirstName: 'Adam',
      sellerLastName: 'Test',
      sellerAvatarUrl: 'avatar.jpg',
      primaryPhotoUrl: 'primary.jpg',
      additionalPhotoUrls: ['extra1.jpg', 'extra2.jpg'],
      videoUrl: 'video.mp4',
      tags: ['angular', 'csharp'],
      packages: [
        {
          id: 'pkg-1',
          tier: 'Basic',
          name: 'Basic',
          description: 'Basic package',
          deliveryDays: 3,
          revisions: 1,
          price: 25,
        },
      ],
      requirements: [
        {
          id: 'req-1',
          type: 'FreeText',
          question: 'What do you need?',
          isRequired: true,
          sortOrder: 0,
          choices: [],
        },
      ],
      averageRating: 5,
      totalReviews: 1,
      reviews: [],
    };

    service.loadFromGig(gig);

    expect(service.editingGigId()).toBe('gig-1');
    expect(service.isEditMode()).toBe(true);

    expect(service.overview()?.title).toBe('Existing gig');
    expect(service.pricing()?.packages[0].price).toBe(25);
    expect(service.requirements()?.requirements[0].question).toBe('What do you need?');

    expect(service.gallery()?.photos[0]).toEqual({
      kind: 'existing',
      url: 'primary.jpg',
      previewUrl: 'primary.jpg',
    });

    expect(service.gallery()?.video).toEqual({
      kind: 'existing',
      url: 'video.mp4',
      previewUrl: 'video.mp4',
    });
  });
});
