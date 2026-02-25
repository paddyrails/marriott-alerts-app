import axios from 'axios';
import { env } from '../utils/env';

const http = axios.create({
  baseURL: env.apiBaseUrl,
  headers: { 'Content-Type': 'application/json' },
});

let logoutCallback: (() => void) | null = null;

export function setLogoutCallback(fn: () => void) {
  logoutCallback = fn;
}

http.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

http.interceptors.response.use(
  (response) => response,
  (error) => {
    if (axios.isAxiosError(error) && error.response?.status === 401) {
      logoutCallback?.();
    }
    return Promise.reject(error);
  },
);

export default http;
