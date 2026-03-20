import { ChangeDetectionStrategy, Component, computed, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {GigRequirementPayload} from '../../../shared/models/gig.model';

export type RequirementType = 'free_text' | 'file_upload' | 'multiple_choice';

export interface RequirementsFormValue {
  requirements: GigRequirementPayload[];
}

export interface RequirementField {
  id: string;
  type: RequirementType;
  question: string;
  required: boolean;
  choices: string[];
}

const REQUIREMENT_TYPE_LABELS: Record<RequirementType, string> = {
  free_text: 'Free Text',
  file_upload: 'File Upload',
  multiple_choice: 'Multiple Choice',
};

let _idCounter = 0;
function generateId(): string {
  return `req_${++_idCounter}`;
}

@Component({
  selector: 'app-requirements',
  imports: [ReactiveFormsModule],
  templateUrl: './requirements.html',
  styleUrl: './requirements.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Requirements {
  private readonly fb = inject(FormBuilder);

  readonly back = output<void>();
  readonly publish = output<RequirementsFormValue>();

  readonly requirements = signal<RequirementField[]>([]);
  readonly showErrors = signal(false);

  readonly newChoiceInputs = signal<Record<string, string>>({});

  readonly addForm = this.fb.nonNullable.group({
    question: ['', [Validators.required, Validators.minLength(5)]],
    type: ['free_text' as RequirementType, Validators.required],
    required: [false],
  });

  readonly requirementTypeLabels = REQUIREMENT_TYPE_LABELS;
  readonly requirementTypes: RequirementType[] = ['free_text', 'file_upload', 'multiple_choice'];

  readonly currentType = signal<RequirementType>('free_text');

  setType(type: RequirementType): void {
    this.addForm.controls.type.setValue(type);
    this.currentType.set(type);
  }

  readonly isFormVisible = signal(false);
  readonly editingId = signal<string | null>(null);

  readonly hasRequirements = computed(() => this.requirements().length > 0);

  readonly choiceBuffer = signal<string[]>([]);

  showAddForm(): void {
    this.isFormVisible.set(true);
    this.editingId.set(null);
    this.addForm.reset({ question: '', type: 'free_text', required: false });
    this.currentType.set('free_text');
    this.choiceBuffer.set([]);
    this.showErrors.set(false);
  }

  cancelAdd(): void {
    this.isFormVisible.set(false);
    this.editingId.set(null);
    this.showErrors.set(false);
  }

  addChoice(): void {
    const inputs = this.newChoiceInputs();
    const value = (inputs['new'] ?? '').trim();
    if (!value) return;
    this.choiceBuffer.update((c) => [...c, value]);
    this.newChoiceInputs.update((m) => ({ ...m, new: '' }));
  }

  removeChoice(index: number): void {
    this.choiceBuffer.update((c) => c.filter((_, i) => i !== index));
  }

  updateNewChoiceInput(value: string): void {
    this.newChoiceInputs.update((m) => ({ ...m, new: value }));
  }

  getNewChoiceInput(): string {
    return this.newChoiceInputs()['new'] ?? '';
  }

  saveRequirement(): void {
    this.showErrors.set(true);
    if (this.addForm.invalid) return;

    const { question, type, required } = this.addForm.getRawValue();

    if (type === 'multiple_choice' && this.choiceBuffer().length < 2) {
      return;
    }

    const editId = this.editingId();

    if (editId) {
      this.requirements.update((list) =>
        list.map((r) =>
          r.id === editId
            ? { ...r, question, type, required, choices: type === 'multiple_choice' ? [...this.choiceBuffer()] : [] }
            : r,
        ),
      );
    } else {
      const newReq: RequirementField = {
        id: generateId(),
        question,
        type,
        required,
        choices: type === 'multiple_choice' ? [...this.choiceBuffer()] : [],
      };
      this.requirements.update((list) => [...list, newReq]);
    }

    this.isFormVisible.set(false);
    this.editingId.set(null);
    this.showErrors.set(false);
    this.addForm.reset({ question: '', type: 'free_text', required: false });
    this.choiceBuffer.set([]);
  }

  editRequirement(req: RequirementField): void {
    this.editingId.set(req.id);
    this.isFormVisible.set(true);
    this.addForm.setValue({ question: req.question, type: req.type, required: req.required });
    this.currentType.set(req.type);
    this.choiceBuffer.set([...req.choices]);
    this.showErrors.set(false);
  }

  removeRequirement(id: string): void {
    this.requirements.update((list) => list.filter((r) => r.id !== id));
  }

  moveUp(index: number): void {
    if (index === 0) return;
    this.requirements.update((list) => {
      const updated = [...list];
      [updated[index - 1], updated[index]] = [updated[index], updated[index - 1]];
      return updated;
    });
  }

  moveDown(index: number): void {
    this.requirements.update((list) => {
      if (index === list.length - 1) return list;
      const updated = [...list];
      [updated[index], updated[index + 1]] = [updated[index + 1], updated[index]];
      return updated;
    });
  }

  private mapType(type: RequirementType): string {
    const map: Record<RequirementType, string> = {
      free_text: 'FreeText',
      file_upload: 'FileUpload',
      multiple_choice: 'MultipleChoice',
    };
    return map[type];
  }

  submitPublish(): void {
    const requirements: GigRequirementPayload[] = this.requirements().map((r, i) => ({
      type: this.mapType(r.type) as any,
      question: r.question,
      isRequired: r.required,
      sortOrder: i,
      choices: r.choices.length > 0 ? r.choices : null,
    }));

    this.publish.emit({ requirements });
  }
}
