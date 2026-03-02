import { ChangeDetectionStrategy, Component, output } from '@angular/core';
import { LucideAngularModule, Mail, Phone, ShieldCheck } from 'lucide-angular';

@Component({
  selector: 'app-security',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [LucideAngularModule],
  templateUrl: './security.html',
  styleUrl: './security.css',
})
export class Security {
  readonly back = output<void>();
  readonly finish = output<void>();

  readonly icons = { ShieldCheck, Mail, Phone };

  onBack(): void {
    this.back.emit();
  }

  onFinish(): void {
    this.finish.emit();
  }
}
