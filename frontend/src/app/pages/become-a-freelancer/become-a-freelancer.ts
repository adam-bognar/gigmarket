import {Component, ChangeDetectionStrategy, signal, inject} from '@angular/core';
import { StepIndicator } from './components/step-indicator/step-indicator';
import {Personal, PersonalFormValue} from './components/personal/personal';
import {Professional, ProfessionalFormValue} from './components/professional/professional';
import {FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {SellerProfileService} from '../../shared/services/seller-profile.service';
import {switchMap} from 'rxjs';
import {SellerDraftService} from '../../shared/services/seller-draft.service';

@Component({
  selector: 'app-become-a-freelancer',
  imports: [StepIndicator, Personal, Professional, FormsModule, RouterLink],
  templateUrl: './become-a-freelancer.html',
  styleUrl: './become-a-freelancer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BecomeAFreelancer {
  private sellerProfileService = inject(SellerProfileService);
  private readonly draft = inject(SellerDraftService);
  private readonly router = inject(Router);

  readonly currentStep = signal(1);
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);

  goBack(): void {
    this.currentStep.update(s => Math.max(1, s - 1));
  }

  onPersonalContinue(data: PersonalFormValue): void {
    this.currentStep.set(2);
  }

  onProfessionalContinue(data: ProfessionalFormValue): void {
    this.onFinish();
  }

  onFinish(): void {
    const personal = this.draft.personal();
    const professional = this.draft.professional();

    if (!personal || !professional) {
      return;
    }

    this.isSubmitting.set(true);
    this.error.set(null);

    this.sellerProfileService.uploadProfilePic(personal.profilePic).pipe(
      switchMap(({ blobPath }) => {

        const payload = {
          firstName: personal.firstName,
          lastName: personal.lastName,
          profilePicUrl: blobPath,
          description: personal.description,
          languageIds: personal.languageNames,
          occupation: {
            occupationName: professional.occupation,
            occupationFromYear: professional.ofrom,
            occupationToYear: professional.oto,
          },
          skills: professional.skills,
          educations: professional.educations.length > 0 ? professional.educations : null,
          certifications: professional.certifications.length > 0 ? professional.certifications : null,
          personalWebsite: professional.personalWebsite,
        };

        return this.sellerProfileService.createProfile(payload);
      }),
    ).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.draft.clearDraft();
        this.router.navigate(['/browse']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      },
    });
  }
}
