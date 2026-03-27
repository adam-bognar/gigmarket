import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, ArrowRight, Palette, Code2, TrendingUp, Video, LucideIconData } from 'lucide-angular';

interface FloatingCard {
  icon: LucideIconData;
  iconClass: string;
  bgClass: string;
  label: string;
  sub: string;
  top: string;
  left: string;
  delay: string;
  duration: string;
}

@Component({
  selector: 'app-cta-banner',
  imports: [RouterLink, LucideAngularModule],
  templateUrl: './cta-banner.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CtaBanner {
  readonly icons = { ArrowRight };

  readonly floatingCards: FloatingCard[] = [
    { icon: Palette,    iconClass: 'text-amber-400',  bgClass: 'bg-amber-400/10',  label: 'Logo Design',     sub: 'Starting at $25',  top: '0%',  left: '5%',  delay: '0s',    duration: '4s' },
    { icon: Code2,      iconClass: 'text-blue-400',   bgClass: 'bg-blue-400/10',   label: 'Web Development', sub: 'Starting at $80',  top: '15%', left: '45%', delay: '0.5s',  duration: '5s' },
    { icon: TrendingUp, iconClass: 'text-green-400',  bgClass: 'bg-green-400/10',  label: 'SEO & Marketing', sub: 'Starting at $50',  top: '55%', left: '10%', delay: '1s',    duration: '6s' },
    { icon: Video,      iconClass: 'text-red-400',    bgClass: 'bg-red-400/10',    label: 'Video Editing',   sub: 'Starting at $45',  top: '68%', left: '48%', delay: '1.5s',  duration: '4.5s' },
  ];
}
