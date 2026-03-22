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

export interface GigSummaryDto {
  id: string;
  title: string;
  primaryPhotoUrl: string;
  startingPrice: number;
  minDeliveryDays: number;
  sellerProfileId: string;
  sellerFirstName: string;
  sellerLastName: string;
  sellerAvatarUrl: string;
  categoryName: string;
  subcategoryName: string;
  averageRating: number;
  totalReviews: number;
  tags: string[];
  status: string;
}

export interface GigDetailDto {
  id: string;
  title: string;
  description: string;
  status: string;
  createdAtUtc: string;
  categoryId: string;
  categoryName: string;
  subcategoryId: string;
  subcategoryName: string;
  sellerProfileId: string;
  sellerFirstName: string;
  sellerLastName: string;
  sellerAvatarUrl: string;
  sellerCountry?: string | null;
  sellerMemberSinceUtc?: string | null;
  sellerOccupation?: string | null;
  primaryPhotoUrl: string;
  additionalPhotoUrls: string[];
  videoUrl: string | null;
  tags: string[];
  packages: GigDetailPackageDto[];
  requirements: GigDetailRequirementDto[];
  averageRating: number;
  totalReviews: number;
  reviews: ReviewDto[];
}

export interface GigDetailPackageDto {
  id: string;
  tier: string;
  name: string;
  description: string;
  deliveryDays: number;
  revisions: number;
  price: number;
}

export interface GigDetailRequirementDto {
  id: string;
  type: string;
  question: string;
  isRequired: boolean;
  sortOrder: number;
  choices: string[];
}

export interface ReviewDto {
  id: string;
  rating: number;
  comment: string;
  createdAtUtc: string;
  reviewerName: string;
  reviewerAvatarUrl: string;
}

export interface GigDto {
  id: string;
  sellerProfileId: string;
  title: string;
  status: string;
  createdAtUtc: string;
}
