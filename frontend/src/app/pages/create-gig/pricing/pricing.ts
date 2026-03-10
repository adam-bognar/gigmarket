import { ChangeDetectionStrategy, Component, computed, output, signal } from '@angular/core';


interface GuideItem {
  label: string;
  done: boolean;
}

type PackageId = 'basic' | 'standard' | 'premium';

interface PricingPackage {
  id: PackageId;
  label: 'Basic' | 'Standard' | 'Premium';
  headerText: string;
  name: string;
  description: string;
  deliveryTime: string;
  revisions: string;
  price: number | null;
}

@Component({
  selector: 'app-pricing',
  imports: [],
  templateUrl: './pricing.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Pricing {
  readonly back = output<void>();
  readonly continue = output<void>();

  readonly showErrors = signal(false);
  readonly showTips = signal(false);
  readonly packageTips = [
    'Make each tier feel like a meaningful upgrade.',
    'Keep your package names short and benefit-led.',
    'Use higher prices as the scope and speed improve.',
  ] as const;
  readonly deliveryOptions = [
    '1 Day Delivery',
    '2 Days Delivery',
    '3 Days Delivery',
    '5 Days Delivery',
    '7 Days Delivery',
    '10 Days Delivery',
    '14 Days Delivery',
  ] as const;

  readonly revisionOptions = ['1', '2', '3', '5', 'Unlimited'] as const;

  readonly packages = signal<PricingPackage[]>([
    {
      id: 'basic',
      label: 'Basic',
      headerText: 'A simple starter option for smaller tasks.',
      name: 'Basic Starter',
      description: 'Core service features for a quick start.',
      deliveryTime: '2 Days Delivery',
      revisions: '1',
      price: 20,
    },
    {
      id: 'standard',
      label: 'Standard',
      headerText: 'A more complete option for growing needs.',
      name: 'Standard Pro',
      description: 'Comprehensive solution for growing needs.',
      deliveryTime: '5 Days Delivery',
      revisions: '3',
      price: 50,
    },
    {
      id: 'premium',
      label: 'Premium',
      headerText: 'Your highest-value package with priority support.',
      name: 'Premium Elite',
      description: 'The ultimate VIP package with priority support.',
      deliveryTime: '10 Days Delivery',
      revisions: 'Unlimited',
      price: 150,
    },
  ]);


  readonly packagesEnabled = signal(true);
  readonly packageSectionTitle = computed(() => (this.packagesEnabled() ? 'Offer Packages' : 'Offer a Single Package'));
  readonly packageSectionDescription = computed(() =>
    this.packagesEnabled()
      ? 'Create three tiers so buyers can pick the scope that fits their needs and budget.'
      : 'Keep one clear package available now, then switch packages on when you are ready to offer tiered options.',
  );
  readonly visiblePackages = computed(() =>
    this.packagesEnabled() ? this.packages() : [this.packages()[0]],
  );
  readonly guideItems = computed<GuideItem[]>(() => {
    const visible = this.visiblePackages();

    return [
      {
        label: this.packagesEnabled()
          ? 'Three package tiers are enabled.'
          : 'Single package mode is enabled.',
        done: true,
      },
      {
        label: 'Each visible package has a short, clear name.',
        done: visible.every((pkg) => !this.isNameInvalid(pkg)),
      },
      {
        label: 'Each visible package has a clear description.',
        done: visible.every((pkg) => !this.isDescriptionInvalid(pkg)),
      },
      {
        label: 'Each visible package has delivery time, revisions, and price.',
        done: visible.every(
          (pkg) => !!pkg.deliveryTime && !!pkg.revisions && !this.isPriceInvalid(pkg),
        ),
      },
    ];

  });

  toggleTips(): void {
    this.showTips.update((value) => !value);
  }

  togglePackages(): void {
    this.packagesEnabled.update((value) => !value);
  }

  updateName(index: number, event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.updatePackage(index, { name: value });
  }

  updateDescription(index: number, event: Event): void {
    const value = (event.target as HTMLTextAreaElement).value;
    this.updatePackage(index, { description: value });
  }

  updateDeliveryTime(index: number, event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.updatePackage(index, { deliveryTime: value });
  }

  updateRevisions(index: number, event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.updatePackage(index, { revisions: value });
  }

  updatePrice(index: number, event: Event): void {
    const rawValue = (event.target as HTMLInputElement).value;
    this.updatePackage(index, {
      price: rawValue === '' ? null : Number(rawValue),
    });
  }

  isNameInvalid(pkg: PricingPackage): boolean {
    const length = pkg.name.trim().length;
    return length < 3 || length > 40;
  }

  isDescriptionInvalid(pkg: PricingPackage): boolean {
    return pkg.description.trim().length < 20;
  }

  isPriceInvalid(pkg: PricingPackage): boolean {
    return pkg.price === null || Number.isNaN(pkg.price) || pkg.price < 5;
  }

  private isPackageInvalid(pkg: PricingPackage): boolean {
    return (
      this.isNameInvalid(pkg) ||
      this.isDescriptionInvalid(pkg) ||
      this.isPriceInvalid(pkg)
    );
  }

  private updatePackage(index: number, patch: Partial<PricingPackage>): void {
    this.packages.update((current) =>
      current.map((pkg, i) => (i === index ? { ...pkg, ...patch } : pkg)),
    );
  }

  submit(): void {
    this.showErrors.set(true);

    const hasInvalidPackage = this.visiblePackages().some((pkg) =>
      this.isPackageInvalid(pkg),
    );

    if (hasInvalidPackage) {
      return;
    }

    console.log('Pricing payload:', this.visiblePackages());

    this.continue.emit();
  }
}
