import {ChangeDetectionStrategy, Component, inject, signal, computed, output, OnInit} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormArray,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import {ChevronDown, LucideAngularModule, Plus, Trash2, X} from 'lucide-angular';
import {SellerDraftService} from '../../../../shared/services/seller-draft.service';

export interface ProfessionalFormValue {
  occupation: string;
  ofrom: number;
  oto: number;
  skills: string[];
  educations: { country: string; institutionName: string; degree: string; major: string; graduationYear: number }[];
  certifications: { name: string; issuingOrganization: string; year: number }[];
  personalWebsite: string | null;
}

function yearRangeValidator(control: AbstractControl): ValidationErrors | null {
  const from = Number(control.get('ofrom')?.value);
  const to = Number(control.get('oto')?.value);
  if (from && to && to < from) {
    return { yearRange: true };
  }
  return null;
}

@Component({
  selector: 'app-professional',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    FormsModule,
    LucideAngularModule,
    ReactiveFormsModule
  ],
  templateUrl: './professional.html',
  styleUrl: './professional.css',
})
export class Professional implements OnInit {
  private fb = inject(FormBuilder);
  private readonly draft = inject(SellerDraftService);

  readonly back = output<void>();
  readonly continue = output<ProfessionalFormValue>();

  private readonly CURRENT_YEAR = 2026;
  readonly icons = { ChevronDown, Plus, Trash2, X };

  years = computed(() => {
    const result: number[] = [];
    for (let y = this.CURRENT_YEAR; y >= 1970; y--) result.push(y);
    return result;
  });

  readonly degreeOptions = [
    'High School Diploma', 'Associate Degree', 'BSc', 'BA', 'BEng',
    'MSc', 'MA', 'MEng', 'MBA', 'PhD', 'MD', 'JD', 'Other'
  ];

  readonly countryOptions = [
    'Afghanistan','Albania','Algeria','Andorra','Angola','Argentina','Armenia','Australia','Austria','Azerbaijan',
    'Bahamas','Bahrain','Bangladesh','Belarus','Belgium','Belize','Benin','Bhutan','Bolivia','Bosnia and Herzegovina',
    'Botswana','Brazil','Brunei','Bulgaria','Burkina Faso','Burundi','Cambodia','Cameroon','Canada','Chad','Chile',
    'China','Colombia','Congo','Costa Rica','Croatia','Cuba','Cyprus','Czech Republic','Denmark','Ecuador','Egypt',
    'El Salvador','Estonia','Ethiopia','Finland','France','Georgia','Germany','Ghana','Greece','Guatemala','Honduras',
    'Hungary','India','Indonesia','Iran','Iraq','Ireland','Israel','Italy','Jamaica','Japan','Jordan','Kazakhstan',
    'Kenya','Kuwait','Kyrgyzstan','Latvia','Lebanon','Libya','Lithuania','Luxembourg','Malaysia','Malta','Mexico',
    'Moldova','Mongolia','Montenegro','Morocco','Mozambique','Myanmar','Nepal','Netherlands','New Zealand','Nicaragua',
    'Nigeria','North Korea','Norway','Oman','Pakistan','Palestine','Panama','Paraguay','Peru','Philippines','Poland',
    'Portugal','Qatar','Romania','Russia','Rwanda','Saudi Arabia','Senegal','Serbia','Singapore','Slovakia','Slovenia',
    'Somalia','South Africa','South Korea','Spain','Sri Lanka','Sudan','Sweden','Switzerland','Syria','Taiwan',
    'Tajikistan','Tanzania','Thailand','Tunisia','Turkey','Uganda','Ukraine','United Arab Emirates','United Kingdom',
    'United States','Uruguay','Uzbekistan','Venezuela','Vietnam','Yemen','Zambia','Zimbabwe'
  ];

  skillInput = signal('');

  form = this.fb.group({
    occupation: ['', Validators.required],
    ofrom: ['', Validators.required],
    oto: ['', Validators.required],
    skills: this.fb.array<string>([], Validators.required),
    education: this.fb.array<ReturnType<typeof this.createEducationGroup>>([]),
    certifications: this.fb.array<ReturnType<typeof this.createCertificationGroup>>([]),
    personalWebsite: [''],
  }, { validators: yearRangeValidator });

  ngOnInit(): void {
    const saved = this.draft.professional();
    if (!saved) return;

    this.form.patchValue({
      occupation: saved.occupation,
      ofrom: String(saved.ofrom),
      oto: String(saved.oto),
      personalWebsite: saved.personalWebsite ?? '',
    });

    this.skillsArray.clear();
    saved.skills.forEach(s => this.skillsArray.push(this.fb.control(s, Validators.required)));

    this.educationArray.clear();
    saved.educations.forEach(e => {
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
    saved.certifications.forEach(c => {
      const group = this.createCertificationGroup();
      group.patchValue({
        name: c.name,
        certifiedFrom: c.issuingOrganization,
        year: String(c.year),
      });
      this.certificationsArray.push(group);
    });
  }

  get yearRangeError() {
    return this.form.errors?.['yearRange'] &&
      this.form.controls.ofrom.touched &&
      this.form.controls.oto.touched;
  }

  get skillsArray(): FormArray {
    return this.form.controls.skills as FormArray;
  }

  get educationArray(): FormArray {
    return this.form.controls.education as FormArray;
  }

  get certificationsArray(): FormArray {
    return this.form.controls.certifications as FormArray;
  }

  addSkill(): void {
    const val = this.skillInput().trim();
    if (!val) return;
    const exists = (this.skillsArray.value as string[]).some(
      (s: string) => s.toLowerCase() === val.toLowerCase()
    );
    if (!exists) {
      this.skillsArray.push(this.fb.control(val, Validators.required));
    }
    this.skillInput.set('');
  }

  removeSkill(index: number): void {
    this.skillsArray.removeAt(index);
  }

  onSkillKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addSkill();
    }
  }

  onSkillInputChange(event: Event): void {
    this.skillInput.set((event.target as HTMLInputElement).value);
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

  addEducation(): void {
    this.educationArray.push(this.createEducationGroup());
  }

  removeEducation(index: number): void {
    this.educationArray.removeAt(index);
  }

  createCertificationGroup() {
    return this.fb.group({
      name: [''],
      certifiedFrom: [''],
      year: [''],
    });
  }

  addCertification(): void {
    this.certificationsArray.push(this.createCertificationGroup());
  }

  removeCertification(index: number): void {
    this.certificationsArray.removeAt(index);
  }

  onBack(): void {
    this.back.emit();
  }

  onContinue(): void {
    this.form.markAllAsTouched();
    if (this.form.valid && this.skillsArray.length > 0) {
      const v = this.form.getRawValue();
      const value: ProfessionalFormValue = {
        occupation: v.occupation!,
        ofrom: Number(v.ofrom),
        oto: Number(v.oto),
        skills: this.skillsArray.value as string[],
        educations: (v.education ?? []).map((e: any) => ({
          country: e.country,
          institutionName: e.institution,
          degree: e.title,
          major: e.major,
          graduationYear: Number(e.graduationYear),
        })),
        certifications: (v.certifications ?? []).map((c: any) => ({
          name: c.name,
          issuingOrganization: c.certifiedFrom,
          year: Number(c.year),
        })),
        personalWebsite: v.personalWebsite || null,
      };
      this.draft.setProfessional(value);
      this.continue.emit(value);
    }
  }

}
