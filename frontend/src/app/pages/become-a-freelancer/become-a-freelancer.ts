import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { StepIndicator } from './components/step-indicator/step-indicator';
import { Personal } from './components/personal/personal';
import { Professional } from './components/professional/professional';
import {Security} from './components/security/security';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-become-a-freelancer',
  imports: [StepIndicator, Personal, Professional, Security, FormsModule, RouterLink],
  templateUrl: './become-a-freelancer.html',
  styleUrl: './become-a-freelancer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BecomeAFreelancer {
  readonly currentStep = signal(3);

  goBack(): void {
    this.currentStep.update(s => Math.max(1, s - 1));
  }

  goNext(): void {
    this.currentStep.update(s => Math.min(4, s + 1));
  }
}

