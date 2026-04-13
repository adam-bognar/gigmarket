import {Component, computed, input, OnInit, output, signal} from '@angular/core';
import {NgOptimizedImage} from '@angular/common';
import {GalleryDraft, PhotoSlot, VideoSlot} from '../../../shared/services/gig-draft.service';

export type GalleryFormValue = GalleryDraft;

const MAX_PHOTOS = 3;
const MAX_VIDEO_SIZE_MB = 75;
const ACCEPTED_IMAGE_TYPES = ['image/jpeg', 'image/png'];
const ACCEPTED_VIDEO_TYPES = ['video/mp4'];

function emptySlots(): PhotoSlot[] {
  return Array.from({ length: MAX_PHOTOS }, (): PhotoSlot => ({ kind: 'empty' }));
}

@Component({
  selector: 'app-gallery',
  imports: [],
  templateUrl: './gallery.html',
  styleUrl: './gallery.css',
})
export class Gallery implements OnInit {
  readonly back = output<void>();
  readonly continue = output<GalleryFormValue>();

  readonly initialValue = input<GalleryDraft | null>(null);

  readonly photos = signal<PhotoSlot[]>(emptySlots());
  readonly video = signal<VideoSlot>({ kind: 'empty' });

  readonly showErrors = signal(false);
  readonly photoError = signal<string | null>(null);
  readonly videoError = signal<string | null>(null);

  // For file input accept attributes
  readonly acceptedImageTypes = ACCEPTED_IMAGE_TYPES.join(',');
  readonly acceptedVideoTypes = ACCEPTED_VIDEO_TYPES.join(',');

  readonly hasPrimaryPhoto = computed(() => this.photos()[0].kind !== 'empty');
  readonly hasVideo = computed(() => this.video().kind !== 'empty');

  readonly maxPhotos = MAX_PHOTOS;

  ngOnInit(): void {
    const draft = this.initialValue();
    if (draft) {
      this.photos.set(draft.photos);
      this.video.set(draft.video);
    }
  }

  readonly videoPreviewUrl = computed(() => {
    const v = this.video();
    return v.kind !== 'empty' ? v.previewUrl : null;
  });

  onPhotoSelected(event: Event, index: number): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.photoError.set(null);

    if (!ACCEPTED_IMAGE_TYPES.includes(file.type)) {
      this.photoError.set('Please upload a JPG, PNG image.');
      input.value = '';
      return;
    }

    const reader = new FileReader();
    reader.onload = (e) => {
      const previewUrl = e.target?.result as string;
      this.photos.update(slots =>
        slots.map((slot, i): PhotoSlot =>
          i === index ? { kind: 'new', file, previewUrl } : slot,
        ),
      );
    };

    reader.readAsDataURL(file);
    input.value = '';
  }

  removePhoto(index: number): void {
    this.photos.update(slots =>
      slots.map((slot, i): PhotoSlot => i === index ? { kind: 'empty' } : slot),
    );
  }

  onVideoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.videoError.set(null);

    if (!ACCEPTED_VIDEO_TYPES.includes(file.type)) {
      this.videoError.set('Please upload an MP4 video file.');
      input.value = '';
      return;
    }

    const sizeMb = file.size / (1024 * 1024);
    if (sizeMb > MAX_VIDEO_SIZE_MB) {
      this.videoError.set(`Video must be under ${MAX_VIDEO_SIZE_MB} seconds / 75 MB.`);
      input.value = '';
      return;
    }

    const previewUrl = URL.createObjectURL(file);
    this.video.set({ kind: 'new', file, previewUrl });
    input.value = '';
  }

  removeVideo(): void {
    const current = this.video();
    if (current.kind === 'new') {
      URL.revokeObjectURL(current.previewUrl);
    }
    this.video.set({ kind: 'empty' });
    this.videoError.set(null);
  }

  submit(): void {
    this.showErrors.set(true);
    if (!this.hasPrimaryPhoto()) return;

    this.continue.emit({
      photos: this.photos(),
      video: this.video(),
    });
  }
}
