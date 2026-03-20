import { ChangeDetectionStrategy, Component, computed, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';

type SubcategoryOption = {
  value: string;
};

type CategoryOption = {
  value: string;
  subcategories: readonly SubcategoryOption[];
};

type GuideItem = {
  label: string;
  done: boolean;
};

const CATEGORY_OPTIONS: readonly CategoryOption[] = [
  {
    value: 'graphics-design',
    subcategories: [
      { value: 'logo-design' },
      { value: 'brand-style-guides' },
      { value: 'social-media-design' },
    ],
  },
  {
    value: 'digital-marketing',
    subcategories: [
      { value: 'social-media-marketing' },
      { value: 'seo' },
      { value: 'email-marketing' },
    ],
  },
  {
    value: 'writing-translation',
    subcategories: [
      { value: 'website-copy' },
      { value: 'blog-posts' },
      { value: 'technical-writing' },
    ],
  },
];

export interface OverviewFormValue {
  title: string;
  category: string;
  subcategory: string;
  tags: string[];
  description: string;
}

const MIN_DESCRIPTION_LENGTH = 120;
const MAX_DESCRIPTION_LENGTH = 1200;
const MAX_TITLE_LENGTH = 80;
const MAX_TAGS = 5;

@Component({
  selector: 'app-overview',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './overview.html',
})
export class Overview {
  private readonly fb = inject(FormBuilder);

  readonly continue = output<OverviewFormValue>();
  readonly minDescriptionLength = MIN_DESCRIPTION_LENGTH;
  readonly maxDescriptionLength = MAX_DESCRIPTION_LENGTH;
  readonly maxTitleLength = MAX_TITLE_LENGTH;
  readonly maxTags = MAX_TAGS;

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(MAX_TITLE_LENGTH)]],
    description: [
      '',
      [Validators.required, Validators.minLength(MIN_DESCRIPTION_LENGTH), Validators.maxLength(MAX_DESCRIPTION_LENGTH)],
    ],
    category: ['', Validators.required],
    subcategory: ['', Validators.required],
    tagInput: [''],
  });

  // Bridge reactive form valueChanges into signals so computed() can track them
  private readonly titleValue = toSignal(this.form.controls.title.valueChanges, { initialValue: '' });
  private readonly descriptionValue = toSignal(this.form.controls.description.valueChanges, { initialValue: '' });
  private readonly categoryValue = toSignal(this.form.controls.category.valueChanges, { initialValue: '' });

  readonly titleLength = computed(() => this.titleValue().length);
  readonly descriptionLength = computed(() => this.descriptionValue().length);

  readonly tags = signal<string[]>([]);

  readonly availableSubcategories = computed(() => {
    const category = this.categoryValue();
    return CATEGORY_OPTIONS.find((option) => option.value === category)?.subcategories ?? [];
  });

  readonly remainingTags = computed(() => this.maxTags - this.tags().length);

  readonly guideItems = computed<GuideItem[]>(() => {
    const title = this.titleValue().trim();

    return [
      {
        label: 'Include relevant keywords',
        done: this.tags().length >= 3,
      },
      {
        label: 'Describe the specific service',
        done: this.descriptionLength() >= MIN_DESCRIPTION_LENGTH,
      },
      {
        label: 'Keep your title short and specific',
        done: title.length >= 10 && title.length <= MAX_TITLE_LENGTH,
      },
    ];
  });

  protected readonly CATEGORY_OPTIONS = CATEGORY_OPTIONS;

  onCategoryChange(): void {
    const hasSelectedSubcategory = this.availableSubcategories().some(
      (subcategory) => subcategory.value === this.form.controls.subcategory.value,
    );

    if (!hasSelectedSubcategory) {
      this.form.controls.subcategory.setValue('');
    }
  }

  addTag(): void {
    const value = this.form.controls.tagInput.value.trim();

    if (!value) {
      return;
    }

    this.tags.update((current) => {
      if (current.includes(value) || current.length >= this.maxTags) {
        return current;
      }

      return [...current, value];
    });

    this.form.controls.tagInput.setValue('');
  }

  removeTag(index: number): void {
    this.tags.update((current) => current.filter((_, currentIndex) => currentIndex !== index));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    this.continue.emit({
      title: v.title,
      category: v.category,
      subcategory: v.subcategory,
      tags: this.tags(),
      description: v.description,
    });
  }
}
