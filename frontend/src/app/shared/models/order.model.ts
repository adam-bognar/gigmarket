export interface OrderDto {
  id: string;
  gigId: string;
  gigTitle: string;
  gigPrimaryPhotoUrl: string;
  packageId: string;
  packageName: string;
  packageTier: string;
  deliveryDays: number;
  totalPrice: number;
  status: 'Pending' | 'Paid' | 'Cancelled';
  createdAtUtc: string;
  paidAtUtc: string | null;
}

export interface CheckoutRequest {
  gigId: string;
  packageId: string;
}

export interface CheckoutResponse {
  sessionUrl: string;
}
