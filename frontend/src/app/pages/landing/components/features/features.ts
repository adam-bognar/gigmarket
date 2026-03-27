import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LucideAngularModule, BadgeDollarSign, Zap, ShieldCheck, LucideIconData } from 'lucide-angular';

interface Feature {
  icon: LucideIconData;
  iconClass: string;
  bgClass: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-features',
  imports: [LucideAngularModule],
  templateUrl: './features.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Features {
  readonly features: Feature[] = [
    {
      icon: BadgeDollarSign,
      iconClass: 'text-blue-500',
      bgClass: 'bg-blue-50',
      title: 'Best for every budget',
      description: 'Find high-quality services at every price point. No hourly rates — just clear, project-based pricing you agree on upfront.',
    },
    {
      icon: Zap,
      iconClass: 'text-amber-500',
      bgClass: 'bg-amber-50',
      title: 'Quality, done quickly',
      description: 'Hire in minutes, not days. Our smart matching surfaces the right freelancers for your project immediately.',
    },
    {
      icon: ShieldCheck,
      iconClass: 'text-primary',
      bgClass: 'bg-primary/10',
      title: 'Protected payments',
      description: 'Your payment is held securely and only released when you approve the work. Zero risk, total peace of mind.',
    },
  ];
}
