import {Component, ChangeDetectionStrategy, signal, inject} from '@angular/core';
import { StepIndicator } from './components/step-indicator/step-indicator';
import {Personal, PersonalFormValue} from './components/personal/personal';
import {Professional, ProfessionalFormValue} from './components/professional/professional';
import {Security} from './components/security/security';
import {FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {SellerProfileService} from '../../shared/services/seller-profile.service';
import {switchMap} from 'rxjs';
import {SellerDraftService} from '../../shared/services/seller-draft.service';

@Component({
  selector: 'app-become-a-freelancer',
  imports: [StepIndicator, Personal, Professional, Security, FormsModule, RouterLink],
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
    this.currentStep.set(3);
  }

  onFinish(): void {
    const personal = this.draft.personal();
    const professional = this.draft.professional();

    console.log('onFinish called');
    console.log('Personal data:', personal);
    console.log('Professional data:', professional);

    if (!personal || !professional) {
      console.error('Missing personal or professional data');
      return;
    }

    this.isSubmitting.set(true);
    this.error.set(null);

    this.sellerProfileService.uploadProfilePic(personal.profilePic).pipe(
      switchMap(({ blobPath }) => {
        console.log('Profile pic uploaded successfully. Blob path:', blobPath);

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

        console.log('Payload being sent to createProfile:', payload);
        return this.sellerProfileService.createProfile(payload);
      }),
    ).subscribe({
      next: () => {
        console.log('Profile created successfully');
        this.isSubmitting.set(false);
        this.draft.clearDraft();
        this.router.navigate(['/browse']);
      },
      error: (err) => {
        console.error('Error occurred:', err);
        console.error('Error message:', err?.error?.message);
        console.error('Full error object:', JSON.stringify(err, null, 2));

        this.isSubmitting.set(false);
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      },
    });
  }
}

