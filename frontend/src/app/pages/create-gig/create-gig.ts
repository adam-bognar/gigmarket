import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { Overview } from './overview/overview';
import { Pricing } from './pricing/pricing';
import { Requirements } from './requirements/requirements';
import {Gallery} from './gallery/gallery';

@Component({
  selector: 'app-create-gig',
  imports: [Overview, Pricing, Requirements, Gallery],
  templateUrl: './create-gig.html',
  styleUrl: './create-gig.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateGig {
  readonly steps = [
    {
      number: 1,
      title: 'Overview',
      description: 'Define your Gig title, category, tags, and description.',
    },
    {
      number: 2,
      title: 'Pricing',
      description: 'Create clear packages with delivery, revisions, and pricing.',
    },
    {
      number: 3,
      title: 'Requirements',
      description: 'Add questions or file requests for buyers to answer when ordering.',
    },
  ] as const;

  readonly currentStep = signal(4);
  readonly maxUnlockedStep = signal(1);


  goBack(): void {
    this.currentStep.update((step) => Math.max(1, step - 1));
  }

  goNext(): void {
    const nextStep = Math.min(this.steps.length, this.currentStep() + 1);

    this.currentStep.set(nextStep);
    this.maxUnlockedStep.update((step) => Math.max(step, nextStep));
  }
}
