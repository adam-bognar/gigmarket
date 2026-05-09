import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import {AbstractControl, FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {Camera, Check, KeyRound, LoaderCircle, LucideAngularModule, User} from 'lucide-angular';
import {AuthService} from '../../shared/services/auth.service';

function passwordMatchValidator(group: AbstractControl) {
  const np = group.get('newPassword')?.value;
  const cp = group.get('confirmPassword')?.value;
  return np && cp && np !== cp ? {passwordMismatch: true} : null;
}

@Component({
  selector: 'app-account',
  imports: [ReactiveFormsModule, LucideAngularModule],
  templateUrl: './account.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Account implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  readonly icons = {Camera, User, KeyRound, Check, LoaderCircle};

  readonly user = this.authService.user;

  readonly avatarPreview = signal<string | null>(null);
  readonly avatarSaving = signal(false);
  readonly avatarSuccess = signal(false);
  readonly avatarError = signal<string | null>(null);

  readonly infoSaving = signal(false);
  readonly infoSuccess = signal(false);
  readonly infoError = signal<string | null>(null);

  readonly pwSaving = signal(false);
  readonly pwSuccess = signal(false);
  readonly pwError = signal<string | null>(null);

  readonly userInitials = computed(() => {
    const u = this.user();
    if (!u) return '';
    return u.customUsername?.charAt(0).toUpperCase() ?? '';
  });

  readonly infoForm = this.fb.group({
    customUsername: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30), Validators.pattern(/^[a-zA-Z0-9_\-]+$/)]],
  });

  readonly pwForm = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/(?=.*[A-Z])(?=.*[0-9])/)]],
    confirmPassword: ['', Validators.required],
  }, {validators: passwordMatchValidator});

  ngOnInit(): void {
    const u = this.user();
    if (u) {
      this.infoForm.patchValue({customUsername: u.customUsername});
      this.avatarPreview.set(u.profileUrl ?? null);
    }
  }

  onAvatarSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => this.avatarPreview.set(e.target?.result as string);
    reader.readAsDataURL(file);

    this.avatarSaving.set(true);
    this.avatarError.set(null);
    this.avatarSuccess.set(false);

    this.authService.uploadProfilePicture(file).subscribe({
      next: () => {
        this.authService.refreshUser().subscribe();
        this.avatarSaving.set(false);
        this.avatarSuccess.set(true);
        setTimeout(() => this.avatarSuccess.set(false), 3000);
      },
      error: (err) => {
        this.avatarSaving.set(false);
        this.avatarError.set(err?.error?.detail ?? 'Failed to upload avatar.');
      },
    });
  }

  onSaveInfo(): void {
    this.infoForm.markAllAsTouched();
    if (this.infoForm.invalid) return;

    this.infoSaving.set(true);
    this.infoError.set(null);
    this.infoSuccess.set(false);

    const {customUsername} = this.infoForm.getRawValue();
    this.authService.updateAccount(customUsername!).subscribe({
      next: () => {
        this.infoSaving.set(false);
        this.infoSuccess.set(true);
        setTimeout(() => this.infoSuccess.set(false), 3000);
      },
      error: (err) => {
        this.infoSaving.set(false);
        this.infoError.set(err?.error?.detail ?? err?.error?.title ?? 'Failed to update account.');
      },
    });
  }

  onChangePassword(): void {
    this.pwForm.markAllAsTouched();
    if (this.pwForm.invalid) return;

    this.pwSaving.set(true);
    this.pwError.set(null);
    this.pwSuccess.set(false);

    const {currentPassword, newPassword} = this.pwForm.getRawValue();
    this.authService.changePassword(currentPassword!, newPassword!).subscribe({
      next: () => {
        this.pwSaving.set(false);
        this.pwSuccess.set(true);
        this.pwForm.reset();
        setTimeout(() => this.pwSuccess.set(false), 3000);
      },
      error: (err) => {
        this.pwSaving.set(false);
        this.pwError.set(err?.error?.detail ?? err?.error?.title ?? 'Failed to change password.');
      },
    });
  }

  get usernameCtrl() { return this.infoForm.controls.customUsername; }
  get currentPwCtrl() { return this.pwForm.controls.currentPassword; }
  get newPwCtrl() { return this.pwForm.controls.newPassword; }
  get confirmPwCtrl() { return this.pwForm.controls.confirmPassword; }
  get pwMismatch() { return this.pwForm.errors?.['passwordMismatch'] && this.confirmPwCtrl.touched; }
}
