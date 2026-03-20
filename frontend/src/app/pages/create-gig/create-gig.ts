import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Overview, OverviewFormValue } from './overview/overview';
import { Pricing, PricingFormValue } from './pricing/pricing';
import { Requirements, RequirementsFormValue } from './requirements/requirements';
import { Gallery, GalleryFormValue } from './gallery/gallery';
import { GigService } from '../../shared/services/gig.service';
import { forkJoin, of, switchMap } from 'rxjs';
import { CreateGigPayload } from '../../shared/models/gig.model';

@Component({
  selector: 'app-create-gig',
  imports: [Overview, Pricing, Requirements, Gallery],
  templateUrl: './create-gig.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateGig {
  private gigService = inject(GigService);

  readonly currentStep = signal(1);
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);

  private overviewData: OverviewFormValue | null = null;
  private pricingData: PricingFormValue | null = null;
  private requirementsData: RequirementsFormValue | null = null;

  goBack(): void {
    this.currentStep.update(s => Math.max(1, s - 1));
  }

  onOverviewContinue(data: OverviewFormValue): void {
    this.overviewData = data;
    this.currentStep.set(2);
  }

  onPricingContinue(data: PricingFormValue): void {
    this.pricingData = data;
    this.currentStep.set(3);
  }

  onRequirementsContinue(data: RequirementsFormValue): void {
    this.requirementsData = data;
    this.currentStep.set(4);
  }

  onGalleryContinue(data: GalleryFormValue): void {
    if (!this.overviewData || !this.pricingData || !this.requirementsData) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const overview = this.overviewData;
    const pricing = this.pricingData;
    const requirements = this.requirementsData;
    const [primaryPhoto, ...additionalPhotos] = data.photos;

    const tempGigId = crypto.randomUUID();
    console.log('=== STARTING GIG CREATION ===');
    console.log('tempGigId:', tempGigId);
    console.log('primaryPhoto:', primaryPhoto?.name, primaryPhoto?.size);
    console.log('additionalPhotos:', additionalPhotos.map(f => f.name));
    console.log('video:', data.video?.name ?? 'none');

    this.gigService.uploadGigPhoto(tempGigId, primaryPhoto).pipe(
      switchMap(({ blobPath: primaryPhotoUrl }) => {
        console.log('=== PRIMARY PHOTO UPLOADED ===');
        console.log('primaryPhotoUrl:', primaryPhotoUrl);

        const additionalUploads$ = additionalPhotos.length > 0
          ? forkJoin(additionalPhotos.map(f => this.gigService.uploadGigPhoto(tempGigId, f)))
          : of([]);

        const videoUpload$ = data.video
          ? this.gigService.uploadGigPhoto(tempGigId, data.video)
          : of(null);

        return forkJoin([additionalUploads$, videoUpload$]).pipe(
          switchMap(([additionalResults, videoResult]) => {
            console.log('=== ALL UPLOADS COMPLETE ===');
            console.log('additionalResults:', additionalResults);
            console.log('videoResult:', videoResult);

            const payload: CreateGigPayload = {
              title: overview.title,
              category: overview.category,
              subcategory: overview.subcategory,
              tags: overview.tags,
              description: overview.description,
              packages: pricing.packages,
              requirements: requirements.requirements.length > 0
                ? requirements.requirements
                : null,
              primaryPhotoUrl,
              additionalPhotoUrls: (additionalResults as { blobPath: string }[])
                .map(r => r.blobPath),
              videoUrl: videoResult ? (videoResult as { blobPath: string }).blobPath : null,
            };

            console.log('=== PAYLOAD BEING SENT ===');
            console.log(JSON.stringify(payload, null, 2));

            return this.gigService.createGig(payload);
          })
        );
      })
    ).subscribe({
      next: (gig) => {
        this.isSubmitting.set(false);
        console.log('=== GIG CREATED SUCCESSFULLY ===', gig);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        console.error('=== FULL ERROR ===', err);
        console.error('=== ERROR STATUS ===', err?.status);
        console.error('=== ERROR BODY ===', err?.error);
        console.error('=== VALIDATION ERRORS ===', JSON.stringify(err?.error?.errors, null, 2));
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      }
    });
  }
}
