import {ChangeDetectionStrategy, Component, inject, signal, computed} from '@angular/core';
import {AbstractControl, FormBuilder, FormsModule, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {ChevronDown, LucideAngularModule} from 'lucide-angular';

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
export class Professional {
  private fb = inject(FormBuilder);

  private currentYear = signal(new Date().getFullYear());
  readonly icons = { ChevronDown };

  years = computed(() => {
    const end = this.currentYear();
    const result: number[] = [];
    for (let y = end; y >= 1970; y--) result.push(y);
    return result;
  });

  form = this.fb.group({
    occupation: ['', Validators.required],
    ofrom: ['', Validators.required],
    oto: ['', Validators.required],
  }, { validators: yearRangeValidator });

  get yearRangeError() {
    return this.form.errors?.['yearRange'] &&
      this.form.controls.ofrom.touched &&
      this.form.controls.oto.touched;
  }
}
