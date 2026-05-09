import {ChangeDetectionStrategy, Component, inject, OnInit, signal} from '@angular/core';
import { Overview, OverviewFormValue } from './overview/overview';
import { Pricing, PricingFormValue } from './pricing/pricing';
// import { Requirements, RequirementsFormValue } from './requirements/requirements';
import { Gallery, GalleryFormValue } from './gallery/gallery';
import { GigService } from '../../shared/services/gig.service';
import { forkJoin, of, switchMap } from 'rxjs';
import {CreateGigPayload, UpdateGigPayload} from '../../shared/models/gig.model';
import {GigDraftService} from '../../shared/services/gig-draft.service';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
  selector: 'app-create-gig',
  imports: [Overview, Pricing, /* Requirements, */ Gallery],
  templateUrl: './create-gig.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateGig implements OnInit {
  private gigService = inject(GigService);
  private readonly draft = inject(GigDraftService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly currentStep = signal(1);
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);
  readonly isLoadingGig = signal(false);

  readonly overviewDraft = this.draft.overview;
  readonly pricingDraft = this.draft.pricing;
  readonly requirementsDraft = this.draft.requirements;
  readonly galleryDraft = this.draft.gallery;
  readonly isEditMode = this.draft.isEditMode;

  ngOnInit(): void {
    const gigId = this.route.snapshot.paramMap.get('id');

    if (gigId) {
      this.isLoadingGig.set(true);
      this.gigService.getGigById(gigId).subscribe({
        next: (gig) => {
          this.draft.loadFromGig(gig);
          this.isLoadingGig.set(false);
        },
        error: () => {
          this.error.set('Could not load the Gig. Please try again.');
          this.isLoadingGig.set(false);
        },
      });
    } else {
      this.draft.clearDraft();
    }
  }

  goBack(): void {
    this.currentStep.update(s => Math.max(1, s - 1));
  }

  onOverviewContinue(data: OverviewFormValue): void {
    this.draft.setOverview(data);
    this.currentStep.set(2);
  }

  onPricingContinue(data: PricingFormValue): void {
    this.draft.setPricing(data);
    this.currentStep.set(3);
  }

  // onRequirementsContinue(data: RequirementsFormValue): void {
  //   this.draft.setRequirements(data);
  //   this.currentStep.set(4);
  // }

  onGalleryContinue(data: GalleryFormValue): void {
    this.draft.setGallery(data);
    this.isEditMode() ? this.submitUpdate() : this.submitCreate();
  }

  private submitCreate(): void {
    const overview = this.draft.overview();
    const pricing = this.draft.pricing();
    // const requirements = this.draft.requirements();
    const gallery = this.draft.gallery();
    if (!overview || !pricing || !gallery) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const tempGigId = crypto.randomUUID();
    const [primarySlot, ...additionalSlots] = gallery.photos;

    if (primarySlot.kind !== 'new') {
      this.error.set('Please upload a primary photo.');
      this.isSubmitting.set(false);
      return;
    }

    this.gigService.uploadGigPhoto(tempGigId, primarySlot.file).pipe(
      switchMap(({ blobPath: primaryPhotoUrl }) => {
        const newAdditional = additionalSlots.filter(
          (s): s is Extract<typeof s, { kind: 'new' }> => s.kind === 'new',
        );

        const additionalUploads$ = newAdditional.length > 0
          ? forkJoin(newAdditional.map(s => this.gigService.uploadGigPhoto(tempGigId, s.file)))
          : of([] as { blobPath: string }[]);

        const videoUpload$ = gallery.video.kind === 'new'
          ? this.gigService.uploadGigPhoto(tempGigId, gallery.video.file)
          : of(null);

        return forkJoin([additionalUploads$, videoUpload$]).pipe(
          switchMap(([additionalResults, videoResult]) => {
            const payload: CreateGigPayload = {
              gigId: tempGigId,
              title: overview.title,
              categoryId: overview.categoryId,
              subcategoryId: overview.subcategoryId,
              tags: overview.tags,
              description: overview.description,
              packages: pricing.packages,
              requirements: null,
              // requirements: requirements.requirements.length > 0 ? requirements.requirements : null,
              primaryPhotoUrl,
              additionalPhotoUrls: (additionalResults as { blobPath: string }[]).map(r => r.blobPath),
              videoUrl: videoResult ? (videoResult as { blobPath: string }).blobPath : null,
            };
            return this.gigService.createGig(payload);
          }),
        );
      }),
    ).subscribe({
      next: (gig) => {
        this.isSubmitting.set(false);
        this.draft.clearDraft();
        this.router.navigate(['/gigs', gig.id]);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      },
    });
  }

  private submitUpdate(): void {
    const gigId = this.draft.editingGigId();
    const overview = this.draft.overview();
    const pricing = this.draft.pricing();
    // const requirements = this.draft.requirements();
    const gallery = this.draft.gallery();
    if (!gigId || !overview || !pricing || !gallery) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const [primarySlot, ...additionalSlots] = gallery.photos;

    const primaryUpload$ = primarySlot.kind === 'new'
      ? this.gigService.uploadGigPhoto(gigId, primarySlot.file)
      : of({ blobPath: (primarySlot as Extract<typeof primarySlot, { kind: 'existing' }>).url });

    const nonEmptyAdditional = additionalSlots.filter(s => s.kind !== 'empty');
    const additionalUploads$ = nonEmptyAdditional.length > 0
      ? forkJoin(
        nonEmptyAdditional.map(s =>
          s.kind === 'new'
            ? this.gigService.uploadGigPhoto(gigId, s.file)
            : of({ blobPath: (s as Extract<typeof s, { kind: 'existing' }>).url }),
        ),
      )
      : of([] as { blobPath: string }[]);

    const videoUpload$ = gallery.video.kind === 'new'
      ? this.gigService.uploadGigPhoto(gigId, gallery.video.file)
      : gallery.video.kind === 'existing'
        ? of({ blobPath: gallery.video.url })
        : of(null);

    forkJoin([primaryUpload$, additionalUploads$, videoUpload$]).pipe(
      switchMap(([primaryResult, additionalResults, videoResult]) => {
        const payload: UpdateGigPayload = {
          title: overview.title,
          categoryId: overview.categoryId,
          subcategoryId: overview.subcategoryId,
          tags: overview.tags,
          description: overview.description,
          packages: pricing.packages,
          requirements: null,
          // requirements: requirements.requirements.length > 0 ? requirements.requirements : null,
          primaryPhotoUrl: (primaryResult as { blobPath: string }).blobPath,
          additionalPhotoUrls: (additionalResults as { blobPath: string }[]).map(r => r.blobPath),
          videoUrl: videoResult ? (videoResult as { blobPath: string }).blobPath : null,
        };
        return this.gigService.updateGig(gigId, payload);
      }),
    ).subscribe({
      next: (gig) => {
        this.isSubmitting.set(false);
        this.draft.clearDraft();
        this.router.navigate(['/gigs', gig.id]);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      },
    });
  }
}
