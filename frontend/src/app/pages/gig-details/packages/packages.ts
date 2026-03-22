import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  signal,
} from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import {GigDetailPackageDto, PackageTier} from '../../../shared/models/gig.model';

@Component({
  selector: 'app-packages',
  imports: [],
  templateUrl: './packages.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppPackages {
  packages = input.required<GigDetailPackageDto[]>();

  activeTab = signal<PackageTier>('Basic');

  readonly tabs: PackageTier[] = ['Basic', 'Standard', 'Premium'];

  activePackage = computed(() => {
    const tab = this.activeTab();
    return this.packages().find((p) => p.tier === tab) ?? this.packages()[0];
  });

  hasTab(tier: PackageTier): boolean {
    return this.packages().some((p) => p.tier === tier);
  }

  setTab(tier: PackageTier) {
    if (this.hasTab(tier)) this.activeTab.set(tier);
  }

  get featuresForDisplay(): Array<{ label: string; included: boolean }> {
    const pkg = this.activePackage();
    if (!pkg) return [];

    const all = [
      { label: 'Source file', tier: 'Premium' },
      { label: 'Vector file', tier: 'Standard' },
      { label: '3D mockup', tier: 'Basic' },
      { label: 'High resolution', tier: 'Basic' },
      { label: 'Logo transparency', tier: 'Basic' },
      { label: 'Concepts included', tier: 'Basic' },
    ];

    const tierOrder: PackageTier[] = ['Basic', 'Standard', 'Premium'];
    const currentTierIdx = tierOrder.indexOf(pkg.tier as PackageTier);

    return all.map((f) => ({
      label: f.label,
      included: tierOrder.indexOf(f.tier as PackageTier) <= currentTierIdx,
    }));
  }

  onContinue() {
    console.log('Continue with package:', this.activePackage());
  }

  onCompare() {
    console.log('Compare packages');
  }
}
