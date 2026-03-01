import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import {ReactiveFormsModule, FormBuilder, Validators, FormArray} from '@angular/forms';
import { inject } from '@angular/core';
import { StepIndicator } from './components/step-indicator/step-indicator';
import {LucideAngularModule, Camera, Trash2, ChevronDown} from 'lucide-angular';
import {Personal} from './components/personal/personal';
import {Professional} from './components/professional/professional';

@Component({
  selector: 'app-become-a-freelancer',
  imports: [StepIndicator, ReactiveFormsModule, LucideAngularModule, Personal, Professional],
  templateUrl: './become-a-freelancer.html',
  styleUrl: './become-a-freelancer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BecomeAFreelancer {
  readonly currentStep = signal(2);
}

