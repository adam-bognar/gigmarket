import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import {
  LucideAngularModule,
  ArrowLeft,
  Clock,
  RefreshCw,
  DollarSign,
  Calendar,
  User,
  UploadCloud,
  CheckCircle,
  RotateCcw,
  Paperclip,
  X,
  FileText,
  Package,
} from 'lucide-angular';
import {OrderService} from '../../shared/services/order.service';
import {AuthService} from '../../shared/services/auth.service';
import {ActivityItem, OrderDeliveryDto, OrderDetailDto, OrderRevisionRequestDto} from '../../shared/models/order.model';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [DatePipe, DecimalPipe, NgClass, RouterLink, LucideAngularModule],
  templateUrl: './order-detail.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderDetail implements OnInit {
  private readonly route   = inject(ActivatedRoute);
  private readonly orderSvc = inject(OrderService);
  private readonly authSvc  = inject(AuthService);

  readonly ArrowLeft    = ArrowLeft;
  readonly Clock        = Clock;
  readonly RefreshCw    = RefreshCw;
  readonly DollarSign   = DollarSign;
  readonly Calendar     = Calendar;
  readonly User         = User;
  readonly UploadCloud  = UploadCloud;
  readonly CheckCircle  = CheckCircle;
  readonly RotateCcw    = RotateCcw;
  readonly Paperclip    = Paperclip;
  readonly X            = X;
  readonly FileText     = FileText;
  readonly Package      = Package;

  readonly order   = signal<OrderDetailDto | null>(null);
  readonly loading = signal(true);
  readonly error   = signal<string | null>(null);

  readonly deliverMessage   = signal('');
  readonly deliverFiles     = signal<{ name: string; url: string }[]>([]);
  readonly uploadingFile    = signal(false);
  readonly submittingDeliver = signal(false);
  readonly deliverError     = signal<string | null>(null);

  readonly revisionMessage    = signal('');
  readonly submittingRevision = signal(false);
  readonly revisionError      = signal<string | null>(null);

  readonly submittingAccept = signal(false);

  readonly currentUser = this.authSvc.user;

  readonly viewerRole = computed<'buyer' | 'seller' | 'unknown'>(() => {
    const o = this.order();
    const u = this.currentUser();
    if (!o || !u) return 'unknown';
    if (u.id === o.buyerUserId)  return 'buyer';
    if (u.id === o.sellerUserId) return 'seller';
    return 'unknown';
  });

  readonly activityFeed = computed<ActivityItem[]>(() => {
    const o = this.order();
    if (!o) return [];
    const deliveries: ActivityItem[] = o.deliveries.map(d => ({ type: 'delivery', data: d }));
    const revisions: ActivityItem[]  = o.revisionRequests.map(r => ({ type: 'revision', data: r }));
    return [...deliveries, ...revisions].sort(
      (a, b) => new Date(b.data.createdAtUtc).getTime() - new Date(a.data.createdAtUtc).getTime()
    );
  });

  readonly daysRemaining = computed(() => {
    const o = this.order();
    if (!o?.deadlineUtc) return null;
    const diff = new Date(o.deadlineUtc).getTime() - Date.now();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  });

  readonly revisionsRemaining = computed(() => {
    const o = this.order();
    if (!o) return 0;
    return Math.max(0, o.revisionsAllowed - o.revisionsUsed);
  });

  readonly canDeliver = computed(() => {
    const o = this.order();
    return this.viewerRole() === 'seller' &&
      (o?.status === 'InProgress' || o?.status === 'UnderRevision');
  });

  readonly canAcceptOrRevise = computed(() => {
    return this.viewerRole() === 'buyer' && this.order()?.status === 'Delivered';
  });

  readonly isCompleted = computed(() => this.order()?.status === 'Completed');

  ngOnInit(): void {
    this.route.paramMap
      .pipe(switchMap(params => {
        const id = params.get('id')!;
        return this.orderSvc.getOrderById(id);
      }))
      .subscribe({
        next: (order) => {
          this.order.set(order);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Could not load order. Please try again.');
          this.loading.set(false);
        },
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const orderId = this.order()!.id;
    this.uploadingFile.set(true);
    this.deliverError.set(null);

    this.orderSvc.uploadDeliveryFile(orderId, file).subscribe({
      next: (res) => {
        this.deliverFiles.update(files => [...files, { name: file.name, url: res.url }]);
        this.uploadingFile.set(false);
      },
      error: () => {
        this.deliverError.set('File upload failed. Please try again.');
        this.uploadingFile.set(false);
      },
    });

    input.value = '';
  }

  removeFile(index: number): void {
    this.deliverFiles.update(files => files.filter((_, i) => i !== index));
  }

  submitDelivery(): void {
    if (!this.deliverMessage().trim()) {
      this.deliverError.set('Please include a message with your delivery.');
      return;
    }

    const orderId = this.order()!.id;
    this.submittingDeliver.set(true);
    this.deliverError.set(null);

    this.orderSvc
      .deliver(orderId, {
        message: this.deliverMessage(),
        fileUrls: this.deliverFiles().map(f => f.url),
      })
      .subscribe({
        next: () => {
          this.deliverMessage.set('');
          this.deliverFiles.set([]);
          this.submittingDeliver.set(false);
          this.refreshOrder();
        },
        error: () => {
          this.deliverError.set('Failed to submit delivery. Please try again.');
          this.submittingDeliver.set(false);
        },
      });
  }

  submitRevision(): void {
    if (!this.revisionMessage().trim()) {
      this.revisionError.set('Please describe what needs to be changed.');
      return;
    }

    const orderId = this.order()!.id;
    this.submittingRevision.set(true);
    this.revisionError.set(null);

    this.orderSvc
      .requestRevision(orderId, { message: this.revisionMessage() })
      .subscribe({
        next: () => {
          this.revisionMessage.set('');
          this.submittingRevision.set(false);
          this.refreshOrder();
        },
        error: () => {
          this.revisionError.set('Failed to request revision. Please try again.');
          this.submittingRevision.set(false);
        },
      });
  }

  acceptDelivery(): void {
    const orderId = this.order()!.id;
    this.submittingAccept.set(true);

    this.orderSvc.acceptDelivery(orderId).subscribe({
      next: () => {
        this.submittingAccept.set(false);
        this.refreshOrder();
      },
      error: () => this.submittingAccept.set(false),
    });
  }

  private refreshOrder(): void {
    const id = this.order()!.id;
    this.orderSvc.getOrderById(id).subscribe(o => this.order.set(o));
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      InProgress:   'In Progress',
      Delivered:    'Delivered',
      UnderRevision:'Under Revision',
      Completed:    'Completed',
      Cancelled:    'Cancelled',
    };
    return map[status] ?? status;
  }

  statusClasses(status: string): string {
    const map: Record<string, string> = {
      InProgress:    'bg-blue-100 text-blue-700',
      Delivered:     'bg-amber-100 text-amber-700',
      UnderRevision: 'bg-orange-100 text-orange-700',
      Completed:     'bg-green-100 text-green-700',
      Cancelled:     'bg-red-100 text-red-700',
    };
    return map[status] ?? 'bg-surface-alt text-muted';
  }

  isDelivery(item: ActivityItem): item is { type: 'delivery'; data: OrderDeliveryDto } {
    return item.type === 'delivery';
  }

  isRevision(item: ActivityItem): item is { type: 'revision'; data: OrderRevisionRequestDto } {
    return item.type === 'revision';
  }

  fileName(url: string): string {
    return url.split('/').pop() ?? url;
  }
}
