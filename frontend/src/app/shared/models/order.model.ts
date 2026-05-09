export type OrderStatus = 'InProgress' | 'Delivered' | 'UnderRevision' | 'Completed' | 'Cancelled';

export interface OrderDeliveryDto {
  id: string;
  message: string;
  fileUrls: string[];
  createdAtUtc: string;
}

export interface OrderRevisionRequestDto {
  id: string;
  message: string;
  createdAtUtc: string;
}

export interface OrderDetailDto {
  id: string;
  gigId: string;
  gigTitle: string;
  gigPrimaryPhotoUrl: string;
  packageId: string;
  packageName: string;
  packageTier: string;
  deliveryDays: number;
  revisionsAllowed: number;
  revisionsUsed: number;
  totalPrice: number;
  status: OrderStatus;
  createdAtUtc: string;
  paidAtUtc: string | null;
  deadlineUtc: string | null;
  buyerUserId: string;
  buyerUsername: string;
  sellerUserId: string;
  sellerProfileId: string;
  sellerFirstName: string;
  sellerLastName: string;
  sellerAvatarUrl: string;
  deliveries: OrderDeliveryDto[];
  revisionRequests: OrderRevisionRequestDto[];
}

export interface OrderSummaryDto {
  id: string;
  gigId: string;
  gigTitle: string;
  gigPrimaryPhotoUrl: string;
  packageId: string;
  packageName: string;
  packageTier: string;
  deliveryDays: number;
  totalPrice: number;
  status: OrderStatus;
  createdAtUtc: string;
  paidAtUtc: string | null;
}

export interface DeliverOrderPayload {
  message: string;
  fileUrls: string[];
}

export interface RequestRevisionPayload {
  message: string;
}

export interface CheckoutRequest {
  gigId: string;
  packageId: string;
}

export interface CheckoutResponse {
  sessionUrl: string;
}

export type ActivityItem =
  | { type: 'delivery'; data: OrderDeliveryDto }
  | { type: 'revision'; data: OrderRevisionRequestDto };
