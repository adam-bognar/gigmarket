export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  customUsername: string;
  email: string;
  password: string;
}

export interface AuthUser {
  id: string;
  customUsername: string;
  email: string;
  isSeller?: boolean;
  profileUrl?: string;
}
