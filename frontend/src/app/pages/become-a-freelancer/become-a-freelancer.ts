import {Component, ChangeDetectionStrategy, signal, inject} from '@angular/core';
import { StepIndicator } from './components/step-indicator/step-indicator';
import {Personal, PersonalFormValue} from './components/personal/personal';
import {Professional, ProfessionalFormValue} from './components/professional/professional';
import {Security} from './components/security/security';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {SellerProfileService} from '../../shared/services/seller-profile.service';
import {switchMap} from 'rxjs';

@Component({
  selector: 'app-become-a-freelancer',
  imports: [StepIndicator, Personal, Professional, Security, FormsModule, RouterLink],
  templateUrl: './become-a-freelancer.html',
  styleUrl: './become-a-freelancer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BecomeAFreelancer {
  private sellerProfileService = inject(SellerProfileService);

  readonly currentStep = signal(1);
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);

  private personalData: PersonalFormValue | null = null;
  private professionalData: ProfessionalFormValue | null = null;

  goBack(): void {
    this.currentStep.update(s => Math.max(1, s - 1));
  }

  onPersonalContinue(data: PersonalFormValue): void {
    this.personalData = data;
    this.currentStep.set(2);
  }

  onProfessionalContinue(data: ProfessionalFormValue): void {
    this.professionalData = data;
    this.currentStep.set(3);
  }

  onFinish(): void {
    if (!this.personalData || !this.professionalData) return;

    this.isSubmitting.set(true);
    this.error.set(null);

    const personal = this.personalData;
    const professional = this.professionalData;

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

        console.log('=== UPLOAD SUCCEEDED ===');
        console.log('blobPath:', blobPath);
        console.log('=== PAYLOAD BEING SENT ===');
        console.log(JSON.stringify(payload, null, 2));

        return this.sellerProfileService.createProfile(payload);
      })
    ).subscribe({
      next: (result) => {
        this.isSubmitting.set(false);
        console.log('=== SUCCESS ===', result);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        console.error('=== FULL ERROR OBJECT ===', err);
        console.error('=== ERROR STATUS ===', err?.status);
        console.error('=== ERROR BODY ===', err?.error);
        console.error('=== VALIDATION ERRORS ===', JSON.stringify(err?.error?.errors, null, 2)); // ← add this
        this.error.set(err?.error?.message ?? 'Something went wrong. Please try again.');
      }
    });
  }
}

