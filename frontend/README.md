# Marriott Alerts — React Frontend

A React 18 + TypeScript frontend with MUI, React Query, and JWT authentication.

## Quick Start

```bash
# Install dependencies
npm install

# Copy environment file
cp .env.example .env

# Start dev server (port 3000, proxies /api to localhost:8080)
npm run dev
```

## Scripts

| Command         | Description                |
|-----------------|----------------------------|
| `npm run dev`   | Start Vite dev server      |
| `npm run build` | Production build           |
| `npm run preview`| Preview production build  |
| `npm test`      | Run tests (Vitest)         |
| `npm run lint`  | Lint with ESLint           |
| `npm run format`| Format with Prettier       |

## Project Structure

```
frontend/
├── src/
│   ├── api/           # Axios HTTP client + API modules
│   ├── components/    # Shared/reusable components
│   ├── contexts/      # React contexts (Auth)
│   ├── hooks/         # Custom hooks (React Query)
│   ├── pages/         # Page components
│   ├── types/         # TypeScript types
│   ├── utils/         # Utilities (env)
│   ├── __tests__/     # Test files
│   ├── App.tsx        # Route definitions
│   ├── main.tsx       # Entry point
│   └── theme.ts       # MUI theme
├── index.html
├── package.json
├── vite.config.ts
└── tsconfig.json
```

## Pages

| Route              | Auth     | Description              |
|--------------------|----------|--------------------------|
| `/login`           | Public   | Login form               |
| `/register`        | Public   | Registration form        |
| `/dashboard`       | Protected| Welcome dashboard        |
| `/alert-defs`      | Protected| Alert definitions list   |
| `/alert-defs/:id`  | Protected| Edit alert definition    |

## Environment Variables

| Variable            | Description             | Default                   |
|---------------------|-------------------------|---------------------------|
| `VITE_API_BASE_URL` | Backend API base URL    | `http://localhost:8080`   |

## Tech Stack

- **React 18** + TypeScript
- **Vite** — build tool
- **MUI** — UI component library
- **React Query (TanStack)** — server state management
- **React Hook Form + Zod** — form handling & validation
- **Axios** — HTTP client with interceptors
- **React Router v6** — routing
- **React Toastify** — toast notifications
- **Vitest + React Testing Library** — testing
