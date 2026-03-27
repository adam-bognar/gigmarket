import {ChangeDetectionStrategy, Component, computed, inject, input, OnInit, output, signal} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import {CategoriesService} from '../../../shared/services/categories.service';
import {GigCategoryDto, GigSubcategoryDto} from '../../../shared/models/gig.model';

export interface OverviewFormValue {
  title: string;
  categoryId: string;
  subcategoryId: string;
  categoryName: string;
  subcategoryName: string;
  tags: string[];
  description: string;
}

type GuideItem = { label: string; done: boolean };

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
export class Overview implements OnInit {
  readonly initialValue = input<OverviewFormValue | null>(null);
  private readonly fb = inject(FormBuilder);
  private readonly categoriesService = inject(CategoriesService);

  readonly continue = output<OverviewFormValue>();
  readonly minDescriptionLength = MIN_DESCRIPTION_LENGTH;
  readonly maxDescriptionLength = MAX_DESCRIPTION_LENGTH;
  readonly maxTitleLength = MAX_TITLE_LENGTH;
  readonly maxTags = MAX_TAGS;

  readonly categories = signal<GigCategoryDto[]>([]);
  readonly subcategories = signal<GigSubcategoryDto[]>([]);
  readonly isLoadingCategories = signal(false);
  readonly isLoadingSubcategories = signal(false);

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(MAX_TITLE_LENGTH)]],
    description: ['', [
      Validators.required,
      Validators.minLength(MIN_DESCRIPTION_LENGTH),
      Validators.maxLength(MAX_DESCRIPTION_LENGTH),
    ]],
    categoryId: ['', Validators.required],
    subcategoryId: ['', Validators.required],
    tagInput: [''],
  });

  private readonly titleValue = toSignal(this.form.controls.title.valueChanges, { initialValue: '' });
  private readonly descriptionValue = toSignal(this.form.controls.description.valueChanges, { initialValue: '' });
  private readonly categoryIdValue = toSignal(this.form.controls.categoryId.valueChanges, { initialValue: '' });

  readonly titleLength = computed(() => this.titleValue().length);
  readonly descriptionLength = computed(() => this.descriptionValue().length);

  readonly tags = signal<string[]>([]);
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

  ngOnInit(): void {
    this.isLoadingCategories.set(true);
    this.categoriesService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.isLoadingCategories.set(false);
        this.seedFromDraft();
      },
      error: () => this.isLoadingCategories.set(false),
    });
  }

  private seedFromDraft(): void {
    const v = this.initialValue();
    if (!v) return;

    this.form.patchValue({
      title: v.title,
      categoryId: v.categoryId,
      description: v.description,
    });
    this.tags.set(v.tags);

    if (v.categoryId) {
      this.isLoadingSubcategories.set(true);
      this.categoriesService.getSubcategories(v.categoryId).subscribe({
        next: (subs) => {
          this.subcategories.set(subs);
          this.isLoadingSubcategories.set(false);
          this.form.patchValue({ subcategoryId: v.subcategoryId });
        },
        error: () => this.isLoadingSubcategories.set(false),
      });
    }
  }

  onCategoryChange(): void {
    const categoryId = this.form.controls.categoryId.value;
    this.form.controls.subcategoryId.setValue('');
    this.subcategories.set([]);

    if (!categoryId) return;

    this.isLoadingSubcategories.set(true);
    this.categoriesService.getSubcategories(categoryId).subscribe({
      next: (subs) => {
        this.subcategories.set(subs);
        this.isLoadingSubcategories.set(false);
      },
      error: () => this.isLoadingSubcategories.set(false),
    });
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
    const selectedCategory = this.categories().find(c => c.id === v.categoryId);
    const selectedSubcategory = this.subcategories().find(s => s.id === v.subcategoryId);

    this.continue.emit({
      title: v.title,
      categoryId: v.categoryId,
      subcategoryId: v.subcategoryId,
      categoryName: selectedCategory?.name ?? '',
      subcategoryName: selectedSubcategory?.name ?? '',
      tags: this.tags(),
      description: v.description,
    });
  }
}
