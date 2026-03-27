import {PersonalFormValue} from '../../pages/become-a-freelancer/components/personal/personal';
import {Injectable, signal} from '@angular/core';
import {ProfessionalFormValue} from '../../pages/become-a-freelancer/components/professional/professional';

@Injectable({ providedIn: 'root' })
export class SellerDraftService {
  readonly personal = signal<PersonalFormValue | null>(null);
  readonly professional = signal<ProfessionalFormValue | null>(null);

  readonly profilePicPreviewUrl = signal<string | null>(null);

  setPersonal(data: PersonalFormValue, previewUrl: string): void {
    this.personal.set(data);
    this.profilePicPreviewUrl.set(previewUrl);
  }

  setProfessional(data: ProfessionalFormValue): void {
    this.professional.set(data);
  }

  clearDraft(): void {
    this.personal.set(null);
    this.professional.set(null);
    this.profilePicPreviewUrl.set(null);
  }
}
