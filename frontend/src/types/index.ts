export interface User {
  id: string;
  email: string;
  name?: string | null;
}

export interface AuthResponse {
  accessToken: string;
  user: User;
}

export interface AlertDef {
  id: string;
  name: string;
  awsAccountId: string;
  maxBillAmount: number;
  alertRecipientEmails: string;
  createdAt: string;
  updatedAt: string;
}

export interface AlertDefCreateRequest {
  name: string;
  awsAccountId: string;
  maxBillAmount: number;
  alertRecipientEmails: string;
}

export interface AlertDefUpdateRequest {
  name?: string;
  awsAccountId?: string;
  maxBillAmount?: number;
  alertRecipientEmails?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  limit: number;
  total: number;
}

export interface ApiError {
  error: {
    code: string;
    message: string;
    details?: unknown;
  };
}
