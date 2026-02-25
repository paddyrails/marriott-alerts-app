import type { AuthResponse } from '../types';
import http from './http';

export async function loginApi(email: string, password: string): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/api/auth/login', { email, password });
  return data;
}

export async function registerApi(
  email: string,
  password: string,
  name?: string,
): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/api/auth/register', { email, password, name });
  return data;
}
