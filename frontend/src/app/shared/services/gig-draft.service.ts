import {computed, Injectable, signal} from '@angular/core';
import {OverviewFormValue} from '../../pages/create-gig/overview/overview';
import {PricingFormValue} from '../../pages/create-gig/pricing/pricing';
import {RequirementsFormValue} from '../../pages/create-gig/requirements/requirements';
import {GigDetailDto, PackageTier, RequirementType} from '../models/gig.model';

export type PhotoSlot =
  | { kind: 'new'; file: File; previewUrl: string }
  | { kind: 'existing'; url: string; previewUrl: string }
  | { kind: 'empty' };

export type VideoSlot =
  | { kind: 'new'; file: File; previewUrl: string }
  | { kind: 'existing'; url: string; previewUrl: string }
  | { kind: 'empty' };

export interface GalleryDraft {
  photos: PhotoSlot[];
  video: VideoSlot;
}

@Injectable({
  providedIn: 'root',
})
export class GigDraftService {
  readonly editingGigId = signal<string | null>(null);
  readonly isEditMode = computed(() => !!this.editingGigId());

  readonly overview = signal<OverviewFormValue | null>(null);
  readonly pricing = signal<PricingFormValue | null>(null);
  readonly requirements = signal<RequirementsFormValue | null>(null);
  readonly gallery = signal<GalleryDraft | null>(null);

  setOverview(data: OverviewFormValue): void {
    this.overview.set(data);
  }

  setPricing(data: PricingFormValue): void {
    this.pricing.set(data);
  }

  setRequirements(data: RequirementsFormValue): void {
    this.requirements.set(data);
  }

  setGallery(data: GalleryDraft): void {
    this.gallery.set(data);
  }

  loadFromGig(gig: GigDetailDto): void {
    this.editingGigId.set(gig.id);

    this.overview.set({
      title: gig.title,
      categoryId: gig.categoryId,
      subcategoryId: gig.subcategoryId,
      categoryName: gig.categoryName,
      subcategoryName: gig.subcategoryName,
      tags: gig.tags,
      description: gig.description,
    });

    this.pricing.set({
      packages: gig.packages.map(p => ({
        tier: p.tier as PackageTier,
        name: p.name,
        description: p.description,
        deliveryDays: p.deliveryDays,
        revisions: p.revisions,
        price: p.price,
      })),
    });

    this.requirements.set({
      requirements: gig.requirements.map((r, i) => ({
        type: r.type as RequirementType,
        question: r.question,
        isRequired: r.isRequired,
        sortOrder: i,
        choices: r.choices.length > 0 ? r.choices : null,
      })),
    });

    const allPhotoUrls = [
      gig.primaryPhotoUrl,
      ...(gig.additionalPhotoUrls ?? []),
    ].filter(Boolean) as string[];

    const MAX_PHOTOS = 3;
    const photoSlots: PhotoSlot[] = Array.from({ length: MAX_PHOTOS }, (_, i) => {
      const url = allPhotoUrls[i];
      return url
        ? { kind: 'existing', url, previewUrl: url }
        : { kind: 'empty' };
    });

    const videoSlot: VideoSlot = gig.videoUrl
      ? { kind: 'existing', url: gig.videoUrl, previewUrl: gig.videoUrl }
      : { kind: 'empty' };

    this.gallery.set({ photos: photoSlots, video: videoSlot });
  }

  clearDraft(): void {
    this.editingGigId.set(null);
    this.overview.set(null);
    this.pricing.set(null);
    this.requirements.set(null);
    this.gallery.set(null);
  }
}
