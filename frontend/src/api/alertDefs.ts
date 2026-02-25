import type {
  AlertDef,
  AlertDefCreateRequest,
  AlertDefUpdateRequest,
  PaginatedResponse,
} from '../types';
import http from './http';

export async function getAlertDefs(
  page = 1,
  limit = 20,
): Promise<PaginatedResponse<AlertDef>> {
  const { data } = await http.get<PaginatedResponse<AlertDef>>('/api/AlertDefs', {
    params: { page, limit },
  });
  return data;
}

export async function getAlertDef(id: string): Promise<AlertDef> {
  const { data } = await http.get<AlertDef>(`/api/AlertDefs/${id}`);
  return data;
}

export async function createAlertDef(payload: AlertDefCreateRequest): Promise<AlertDef> {
  const { data } = await http.post<AlertDef>('/api/AlertDefs', payload);
  return data;
}

export async function updateAlertDef(id: string, payload: AlertDefUpdateRequest): Promise<AlertDef> {
  const { data } = await http.patch<AlertDef>(`/api/AlertDefs/${id}`, payload);
  return data;
}

export async function deleteAlertDef(id: string): Promise<void> {
  await http.delete(`/api/AlertDefs/${id}`);
}

export async function executeAlertDef(id: string): Promise<void> {
  await http.post(`/api/AlertDefs/${id}/execute`);
}
