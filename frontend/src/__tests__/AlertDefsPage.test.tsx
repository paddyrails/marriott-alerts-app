import { ThemeProvider, createTheme } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { describe, expect, it, vi } from 'vitest';
import AlertDefsPage from '../pages/AlertDefsPage';

vi.mock('../contexts/AuthContext', () => ({
  useAuth: () => ({
    isAuthenticated: true,
    isLoading: false,
    user: { id: '1', email: 'test@test.com' },
    token: 'mock-token',
    login: vi.fn(),
    register: vi.fn(),
    logout: vi.fn(),
  }),
}));

vi.mock('../hooks/useAlertDefs', () => ({
  useAlertDefsList: () => ({
    data: { items: [], page: 1, limit: 20, total: 0 },
    isLoading: false,
    isError: false,
    refetch: vi.fn(),
  }),
  useCreateAlertDef: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
  useExecuteAlertDef: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

function renderWithProviders(ui: React.ReactElement) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <ThemeProvider theme={createTheme()}>
      <BrowserRouter>
        <QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>
      </BrowserRouter>
    </ThemeProvider>,
  );
}

describe('AlertDefsPage', () => {
  it('renders empty state when no alert defs exist', () => {
    renderWithProviders(<AlertDefsPage />);
    expect(screen.getByText(/no alert definitions yet/i)).toBeInTheDocument();
  });

  it('renders create button', () => {
    renderWithProviders(<AlertDefsPage />);
    expect(screen.getByRole('button', { name: /create alert def/i })).toBeInTheDocument();
  });
});
