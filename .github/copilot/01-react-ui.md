# Frontend Build Instructions (Cursor IDE) — React + TypeScript

## Goal
Generate a production-ready **React** frontend that works with a backend API (Spring Boot or any REST API), including:
- Login/Register (JWT)
- Protected routes
- Task CRUD UI (Create/List/View/Edit/Delete)
- Form validation
- API client with token handling
- Clean UI layout + reusable components
- Unit/integration tests
- Docker support (optional)

---

## Target Stack
- React 18
- TypeScript
- Vite
- Routing: react-router-dom
- API: Axios (or Fetch)
- State: React Query (TanStack Query)
- Forms: react-hook-form + zod
- UI: MUI (or Tailwind + shadcn/ui—pick one)
- Notifications: react-toastify (or MUI Snackbar)
- Testing: Vitest + React Testing Library
- Lint/format: ESLint + Prettier

> Default choice: **MUI + React Query + Axios + React Hook Form + Zod**.

---

## What Cursor Should Produce
A frontend app that:
1. Runs locally with one command (`npm i && npm run dev`)
2. Has pages for Login/Register, Task List, Task Detail/Edit
3. Stores JWT securely in memory + localStorage (basic) and attaches it to requests
4. Handles 401 by redirecting to login
5. Shows loading + error states consistently
6. Has reusable components and clean folder structure
7. Includes a small test suite
8. Includes `.env.example` and clear README steps



---

## Environment Variables
Create `.env.example`:

- `VITE_API_BASE_URL=http://localhost:8080`

Use `src/utils/env.ts` to read and validate env values.

---

## Backend API Contract (assume this)
Base URL: `VITE_API_BASE_URL`

### Auth
- `POST /api/auth/register`
  - Body: `{ "email": string, "password": string, "name"?: string }`
- `POST /api/auth/login`
  - Body: `{ "email": string, "password": string }`
  - Response: `{ "accessToken": string, "user": { "id": string, "email": string, "name"?: string } }`

### Tasks (JWT protected)
Header: `Authorization: Bearer <token>`

- `POST /api/alert-defs`
  - Body `
        {
        "name": "string",
        "awsAccountId": "string",
        "maxBillAmount": 0,
        "alertRecipientEmails": "string"
        }
  `
- `GET /api/alert-defs?page=1&limit=20`  
  - Response: `{
        "items": [
            {
            "id": "47206898-ecb6-4d4c-8b38-31f3c3e4f5d2",
            "name": "test-alert",
            "awsAccountId": "457637694252",
            "maxBillAmount": 1,
            "alertRecipientEmails": "test@test.com",
            "createdAt": "2026-02-25T06:24:03.66899+00:00",
            "updatedAt": "2026-02-25T06:24:03.668991+00:00"
            }
        ],
        "page": 1,
        "limit": 20,
        "total": 1
        }`
- `GET /api/alert-defs/:id` 
    - Response: `{
        "id": "47206898-ecb6-4d4c-8b38-31f3c3e4f5d2",
        "name": "test-alert",
        "awsAccountId": "457637694252",
        "maxBillAmount": 1,
        "alertRecipientEmails": "test@test.com",
        "createdAt": "2026-02-25T06:24:03.66899+00:00",
        "updatedAt": "2026-02-25T06:24:03.668991+00:00"
        }`
- `PATCH /api/alert-defs/:id`
  - Body: `{
        "name": "string",
        "awsAccountId": "string",
        "maxBillAmount": 0,
        "alertRecipientEmails": "string"
        }`
  - Response: {
        "id": "47206898-ecb6-4d4c-8b38-31f3c3e4f5d2",
        "name": "test-alert2",
        "awsAccountId": "54745686486",
        "maxBillAmount": 2,
        "alertRecipientEmails": "test@test.com",
        "createdAt": "2026-02-25T06:24:03.66899+00:00",
        "updatedAt": "2026-02-25T17:00:41.9389301+00:00"
        }
- `DELETE /v1/alert-defs/:id` → 204
- `POST /v1/alert-defs/{id}/execute` to Execute an Alert Def

Task model:
```ts
type AlertDef = {
    id: string;
    name: string;
    awsAccountId: string;
    maxBillAmount: 0;
    alertRecipientEmails: string;  
};
```

## App Behavior Requirements
### Routing

/login (public)

/register (public)

/dashboard (protected)

/alert-defs (protected)

/alert-def/:id (protected)

* → NotFound

### Auth Handling

Implement AuthContext providing:

user

token

login(email, password)

register(email, password, name?)

logout()

### Token storage:

Store in localStorage for persistence (simple approach)

Also keep in memory state for immediate use

401 handling:

Axios interceptor should catch 401 and call logout() and redirect to /login

### UI Requirements (MUI)

AppShell layout with top nav

Show logged-in user + Logout button

Alert-Defs page:

Table listing Alert Definitions (Name, AWSAccountId, MaxAmount, Button to execute)

Create Alert Definition button opens modal with new form

Row click opens detail page

Alert Def Detail page:

Edit form (Name, AWSAccountId, MaxAmount, alertRecipients)

Save button

Delete button with confirm dialog

Form Validation

Use react-hook-form + zod:

Email format validation

Password min length 8

Task title required, max 200

### Data Fetching

Use React Query:

useQuery for list and detail

useMutation for create/update/delete

Invalidate queries after mutation

### UX

Consistent loading spinner component

Error state component with retry

Toast notifications for success/failure

Disable buttons during API calls

Show empty state when no tasks exist

API Client Requirements

src/api/http.ts

Create Axios instance with baseURL from env

Request interceptor adds Authorization header if token exists

Response interceptor handles 401 globally

Return typed responses and centralize error shaping.

### Tests (Minimum)

Using Vitest + React Testing Library:

Auth: login page renders, can submit form (mock API)

ProtectedRoute: redirects to /login when not authenticated

Tasks: renders empty state when API returns no items (mock API)

Mock API using MSW or axios mocking (pick one, prefer MSW if comfortable).

### Scripts (package.json)

Include:

dev

build

preview

test

lint

format

## Cursor Execution Steps (Generate Code)

Initialize Vite React TS project under frontend/

Create folder structure exactly as specified

Implement AuthContext + token storage + axios interceptors

Implement routes + protected routes

Implement pages and components

Implement API modules and types

Add MUI theme and AppShell layout

Add tests and ensure npm test passes

Add README with setup steps

### Acceptance Checklist

 npm run dev starts app

 Can register and login to receive token

 Protected routes require login

 Tasks list loads and displays tasks

 Can create, edit, and delete tasks

 401 redirects to login

 Loading/error/empty states work

 npm test passes

 .env.example + README included

## Notes

Keep components small and reusable.

Keep API logic out of UI components (use hooks/services).

Use TypeScript types everywhere (no any).

Don’t hardcode URLs—use env.

