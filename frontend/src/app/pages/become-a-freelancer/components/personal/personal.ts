import {Component, inject, signal} from '@angular/core';
import {FormArray, FormBuilder, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {Camera, ChevronDown, LucideAngularModule, Trash2} from "lucide-angular";

@Component({
  selector: 'app-personal',
    imports: [
        FormsModule,
        LucideAngularModule,
        ReactiveFormsModule
    ],
  templateUrl: './personal.html',
  styleUrl: './personal.css',
})
export class Personal {
  private fb = inject(FormBuilder);

  readonly icons = { Camera, Trash2, ChevronDown };
  readonly maxDescriptionLength = 1000;
  readonly minDescriptionLength = 150;
  readonly descriptionLength = signal(0);
  readonly profilePicPreview = signal<string | null>(null);

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    description: ['', [Validators.required, Validators.minLength(150), Validators.maxLength(1000)]],
    profilePic: [null as File | null, Validators.required],
    languages: this.fb.array([
      this.fb.control('', Validators.required),
    ])
  });

  readonly availableLanguages = [
    'English',
    'Spanish',
    'French',
  ];

  get languages() {
    return this.form.controls.languages as FormArray;
  }

  addLanguage(): void {
    this.languages.push(this.fb.control('', Validators.required));
  }

  removeLanguage(index: number): void {
    this.languages.removeAt(index);
  }

  onDescriptionInput(event: Event): void {
    const value = (event.target as HTMLTextAreaElement).value;
    this.descriptionLength.set(value.length);
  }

  onFileSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0] ?? null;
    this.form.controls.profilePic.setValue(file);
    this.form.controls.profilePic.markAsTouched();
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => this.profilePicPreview.set(e.target?.result as string);
      reader.readAsDataURL(file);
    } else {
      this.profilePicPreview.set(null);
    }
  }
}
