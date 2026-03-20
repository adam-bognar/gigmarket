export type PackageTier = 'Basic' | 'Standard' | 'Premium';
export type RequirementType = 'FreeText' | 'FileUpload' | 'MultipleChoice';

export interface GigPackagePayload {
  tier: PackageTier;
  name: string;
  description: string;
  deliveryDays: number;
  revisions: number;
  price: number;
}

export interface GigRequirementPayload {
  type: RequirementType;
  question: string;
  isRequired: boolean;
  sortOrder: number;
  choices: string[] | null;
}

export interface CreateGigPayload {
  title: string;
  category: string;
  subcategory: string;
  tags: string[];
  description: string;
  packages: GigPackagePayload[];
  requirements: GigRequirementPayload[] | null;
  primaryPhotoUrl: string;
  additionalPhotoUrls: string[] | null;
  videoUrl: string | null;
}
