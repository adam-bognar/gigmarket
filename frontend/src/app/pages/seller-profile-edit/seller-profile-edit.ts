import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {Camera, ChevronDown, LucideAngularModule, Plus, Trash2, X} from 'lucide-angular';
import {switchMap, of} from 'rxjs';
import {SellerProfileService} from '../../shared/services/seller-profile.service';
import {LanguageOption} from '../../shared/models/seller.model';
import {buildYearList, COUNTRY_OPTIONS, DEGREE_OPTIONS, yearRangeValidator} from '../../shared/utils/form.utils';

@Component({
  selector: 'app-seller-profile-edit',
  imports: [ReactiveFormsModule, LucideAngularModule, RouterLink],
  templateUrl: './seller-profile-edit.html',
  styleUrl: './seller-profile-edit.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SellerProfileEdit implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly sellerService = inject(SellerProfileService);
  private readonly router = inject(Router);

  readonly icons = { Camera, ChevronDown, Plus, Trash2, X };

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly error = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly profilePicPreview = signal<string | null>(null);
  readonly availableLanguages = signal<LanguageOption[]>([]);
  readonly skillInput = signal('');

  private existingProfilePicUrl = '';
  private selectedProfilePicFile: File | null = null;

  readonly years = computed(() => buildYearList());
  readonly degreeOptions = DEGREE_OPTIONS;
  readonly countryOptions = COUNTRY_OPTIONS;

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    description: ['', [Validators.required, Validators.minLength(150), Validators.maxLength(1000)]],
    languages: this.fb.array([this.fb.control('', Validators.required)]),

    occupation: ['', Validators.required],
    ofrom: ['', Validators.required],
    oto: ['', Validators.required],
    skills: this.fb.array<string>([], Validators.required),
    education: this.fb.array<ReturnType<typeof this.createEducationGroup>>([]),
    certifications: this.fb.array<ReturnType<typeof this.createCertificationGroup>>([]),
    personalWebsite: [''],
  }, { validators: yearRangeValidator });


  get languages(): FormArray { return this.form.controls.languages as FormArray; }
  get skillsArray(): FormArray { return this.form.controls.skills as FormArray; }
  get educationArray(): FormArray { return this.form.controls.education as FormArray; }
  get certificationsArray(): FormArray { return this.form.controls.certifications as FormArray; }

  get yearRangeError(): boolean {
    return !!(this.form.errors?.['yearRange'] &&
      this.form.controls.ofrom.touched &&
      this.form.controls.oto.touched);
  }

  get descriptionLength(): number {
    return this.form.controls.description.value?.length ?? 0;
  }


  ngOnInit(): void {
    this.sellerService.getLanguages().subscribe(langs => {
      this.availableLanguages.set(langs);

      this.sellerService.getMyProfile().subscribe({
        next: (profile) => {
          this.existingProfilePicUrl = profile.profileImageUrl;
          this.profilePicPreview.set(profile.profileImageUrl);

          this.form.patchValue({
            firstName: profile.firstName,
            lastName: profile.lastName,
            description: profile.description,
            occupation: profile.occupation.name,
            ofrom: String(profile.occupation.fromYear),
            oto: String(profile.occupation.toYear),
            personalWebsite: profile.personalWebsite ?? '',
          });

          this.languages.clear();
          const langIds = profile.languages.length > 0
            ? profile.languages.map(l => l.id)
            : [''];
          langIds.forEach(id => this.languages.push(this.fb.control(id, Validators.required)));

          this.skillsArray.clear();
          profile.skills.forEach(s => this.skillsArray.push(this.fb.control(s, Validators.required)));

          this.educationArray.clear();
          profile.educations.forEach(e => {
            const group = this.createEducationGroup();
            group.patchValue({
              country: e.country,
              institution: e.institutionName,
              title: e.degree,
              major: e.major,
              graduationYear: String(e.graduationYear),
            });
            this.educationArray.push(group);
          });

          this.certificationsArray.clear();
          profile.certifications.forEach(c => {
            const group = this.createCertificationGroup();
            group.patchValue({
              name: c.name,
              certifiedFrom: c.issuingOrganization,
              year: String(c.year),
            });
            this.certificationsArray.push(group);
          });

          this.isLoading.set(false);
        },
        error: () => {
          this.error.set('Failed to load profile. Please try again.');
          this.isLoading.set(false);
        },
      });
    });
  }


  onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.selectedProfilePicFile = file;
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => this.profilePicPreview.set(e.target?.result as string);
      reader.readAsDataURL(file);
    }
  }

  addLanguage(): void {
    this.languages.push(this.fb.control('', Validators.required));
  }

  removeLanguage(index: number): void {
    this.languages.removeAt(index);
  }

  addSkill(): void {
    const val = this.skillInput().trim();
    if (!val) return;
    const exists = (this.skillsArray.value as string[]).some(
      s => s.toLowerCase() === val.toLowerCase()
    );
    if (!exists) this.skillsArray.push(this.fb.control(val, Validators.required));
    this.skillInput.set('');
  }

  removeSkill(index: number): void {
    this.skillsArray.removeAt(index);
  }

  onSkillInputChange(event: Event): void {
    this.skillInput.set((event.target as HTMLInputElement).value);
  }

  onSkillKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addSkill();
    }
  }

  createEducationGroup() {
    return this.fb.group({
      country: [''],
      institution: [''],
      title: [''],
      major: [''],
      graduationYear: [''],
    });
  }

  addEducation(): void { this.educationArray.push(this.createEducationGroup()); }
  removeEducation(index: number): void { this.educationArray.removeAt(index); }

  createCertificationGroup() {
    return this.fb.group({
      name: [''],
      certifiedFrom: [''],
      year: [''],
    });
  }

  addCertification(): void { this.certificationsArray.push(this.createCertificationGroup()); }
  removeCertification(index: number): void { this.certificationsArray.removeAt(index); }

  onSave(): void {
    if (this.isSaving()) return;
    this.form.markAllAsTouched();
    if (this.form.invalid || this.skillsArray.length === 0) return;

    this.isSaving.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    const uploadPic$ = this.selectedProfilePicFile
      ? this.sellerService.uploadProfilePic(this.selectedProfilePicFile)
      : of({ blobPath: this.existingProfilePicUrl });

    uploadPic$.pipe(
      switchMap(({ blobPath }) => {
        const v = this.form.getRawValue();
        return this.sellerService.updateProfile({
          firstName: v.firstName!,
          lastName: v.lastName!,
          description: v.description!,
          profilePicUrl: blobPath,
          languageIds: v.languages as string[],
          occupation: {
            occupationName: v.occupation!,
            occupationFromYear: Number(v.ofrom),
            occupationToYear: Number(v.oto),
          },
          skills: this.skillsArray.value as string[],
          educations: (v.education ?? []).length > 0
            ? (v.education ?? []).map((e: any) => ({
              country: e.country,
              institutionName: e.institution,
              degree: e.title,
              major: e.major,
              graduationYear: Number(e.graduationYear),
            }))
            : null,
          certifications: (v.certifications ?? []).length > 0
            ? (v.certifications ?? []).map((c: any) => ({
              name: c.name,
              issuingOrganization: c.certifiedFrom,
              year: Number(c.year),
            }))
            : null,
          personalWebsite: v.personalWebsite || null,
        });
      })
    ).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.selectedProfilePicFile = null;
        this.successMessage.set('Profile updated successfully.');
        setTimeout(() => this.successMessage.set(null), 4000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.error.set(err?.error?.detail ?? err?.error?.message ?? 'Something went wrong. Please try again.');
      },
    });
  }
}
