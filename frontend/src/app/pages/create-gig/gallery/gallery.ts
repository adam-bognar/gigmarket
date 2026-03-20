import {Component, computed, output, signal} from '@angular/core';
import {NgOptimizedImage} from '@angular/common';


interface GigPhoto {
  file: File | null;
  previewUrl: string | null;
}

export interface GalleryFormValue {
  photos: File[];
  video: File | null;
}

const MAX_PHOTOS = 3;
const MAX_VIDEO_SIZE_MB = 75;
const ACCEPTED_IMAGE_TYPES = ['image/jpeg', 'image/png'];
const ACCEPTED_VIDEO_TYPES = ['video/mp4'];

@Component({
  selector: 'app-gallery',
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './gallery.html',
  styleUrl: './gallery.css',
})
export class Gallery {
  readonly back = output<void>();
  readonly continue = output<GalleryFormValue>();

  readonly photos = signal<GigPhoto[]>([
    {file: null, previewUrl: null},
    {file: null, previewUrl: null},
    {file: null, previewUrl: null},
  ]);

  readonly video = signal<GigPhoto>({
    file: null,
    previewUrl: null,
  });

  readonly showErrors = signal(false);
  readonly photoError = signal<string | null>(null);
  readonly videoError = signal<string | null>(null);

  readonly acceptedImageTypes = ACCEPTED_IMAGE_TYPES.join(',');
  readonly acceptedVideoTypes = ACCEPTED_VIDEO_TYPES.join(',');

  readonly primaryPhoto = computed(() => this.photos()[0]);
  readonly hasVideo = computed(() => !!this.video().file);
  readonly hasPrimaryPhoto = computed(() => !!this.photos()[0].file);

  readonly maxPhotos = MAX_PHOTOS;

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
      this.photos.update((current) =>
        current.map((photo, i) =>
          i === index ? { file, previewUrl: e.target?.result as string } : photo,
        ),
      );
    };

    reader.readAsDataURL(file);
    input.value = '';
  }

  removePhoto(index: number): void {
    this.photos.update((current) =>
      current.map((photo, i) => (i === index ? { file: null, previewUrl: null } : photo)),
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
      this.videoError.set(`Video must be under ${MAX_VIDEO_SIZE_MB} seconds / 50 MB.`);
      input.value = '';
      return;
    }

    const url = URL.createObjectURL(file);
    this.video.set({ file, previewUrl: url });
    input.value = '';
  }

  removeVideo(): void {
    const current = this.video();
    if (current.previewUrl) {
      URL.revokeObjectURL(current.previewUrl);
    }
    this.video.set({ file: null, previewUrl: null });
    this.videoError.set(null);
  }

  submit(): void {
    this.showErrors.set(true);
    if (!this.hasPrimaryPhoto()) return;

    const photoFiles = this.photos()
      .filter(p => p.file !== null)
      .map(p => p.file!);

    this.continue.emit({
      photos: photoFiles,
      video: this.video().file,
    });
  }
}
