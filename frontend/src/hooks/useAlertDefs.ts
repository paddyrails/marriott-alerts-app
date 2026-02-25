import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  createAlertDef,
  deleteAlertDef,
  executeAlertDef,
  getAlertDef,
  getAlertDefs,
  updateAlertDef,
} from '../api/alertDefs';
import type { AlertDefCreateRequest, AlertDefUpdateRequest } from '../types';

export function useAlertDefsList(page = 1, limit = 20) {
  return useQuery({
    queryKey: ['alertDefs', page, limit],
    queryFn: () => getAlertDefs(page, limit),
  });
}

export function useAlertDef(id: string) {
  return useQuery({
    queryKey: ['alertDef', id],
    queryFn: () => getAlertDef(id),
    enabled: !!id,
  });
}

export function useCreateAlertDef() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: AlertDefCreateRequest) => createAlertDef(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alertDefs'] });
    },
  });
}

export function useUpdateAlertDef(id: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: AlertDefUpdateRequest) => updateAlertDef(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alertDefs'] });
      queryClient.invalidateQueries({ queryKey: ['alertDef', id] });
    },
  });
}

export function useDeleteAlertDef() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteAlertDef(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alertDefs'] });
    },
  });
}

export function useExecuteAlertDef() {
  return useMutation({
    mutationFn: (id: string) => executeAlertDef(id),
  });
}
