import {Component, computed, input} from '@angular/core';

@Component({
  selector: 'app-step-indicator',
  imports: [],
  templateUrl: './step-indicator.html',
  styleUrl: './step-indicator.css',
})
export class StepIndicator {
  step = input.required<number>()
  label = input.required<string>()

  currentStep = input.required<number>();

  status = computed(() => {
    if (this.currentStep() > this.step()) {
      return 'completed';
    } else if (this.currentStep() === this.step()) {
      return 'active';
    } else {
      return 'upcoming';
    }
  })
}
