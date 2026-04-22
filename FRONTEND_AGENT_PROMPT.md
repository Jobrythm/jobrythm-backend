You are implementing the frontend client for the `Jobrythm` backend API.

Goal: deliver a production-ready frontend integration with complete API coverage, robust auth/session handling, consistent error UX, and typed request/response models.

Context:
- Backend base URL (local): `http://localhost:8080`
- Auth: JWT Bearer access token + refresh token flow
- Protected routes require header: `Authorization: Bearer <accessToken>`
- API reference source of truth: `api-endpoints.txt`

Hard requirements:
1. Build a typed API layer (single client abstraction) used by all UI features.
2. Implement silent token refresh with request retry and concurrency safety (single in-flight refresh lock).
3. Implement global error normalization and user-facing handling by status code.
4. Implement all endpoints documented in `api-endpoints.txt`, including file upload and PDF/download endpoints.
5. Implement strict ownership/scoping assumptions: all list/detail resources are per authenticated user.
6. Do not hardcode enums/statuses outside shared API types—derive from backend responses where possible.
7. Ensure forms, validation, and payloads exactly match backend request contracts.

Implementation plan:

## 1) API Foundation

Create:
- `api/client` module for HTTP transport
- `api/types` for DTO types
- `api/endpoints` grouping per domain (`auth`, `users`, `clients`, `jobs`, `lineItems`, `quotes`, `invoices`, `billing`, `dashboard`)
- `api/errors` for normalized error shape

Transport rules:
- Attach bearer token on protected requests.
- If response is `401` and request is retryable, attempt refresh once, then retry original request.
- Refresh endpoint: `POST /api/auth/refresh` with `{ userId, refreshToken }`.
- If refresh fails, clear session and redirect to login.
- Prevent refresh stampede:
  - queue/wait pending requests while one refresh is in progress.
  - resolve all queued requests after refresh success/failure.
- Timeouts + cancellation supported.

## 2) Auth & Session

Endpoints:
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout` (authorized)

Session model:
- Store `accessToken`, `refreshToken`, `expiresAt`, `userId`, `email`, `fullName`.
- Parse `expiresAt` and proactively refresh shortly before expiry.
- On logout, call backend logout with `refreshToken`, then clear local session regardless of call outcome.

Validation/UX:
- Registration password policy: min 8, upper, lower, digit, special.
- Handle auth rate limiting (`429`) with clear message + retry delay.

## 3) Users

Endpoints:
- `GET /api/users/me`
- `PUT /api/users/me`
- `POST /api/users/me/logo` (`multipart/form-data`, field `file`, max 5MB, jpg/jpeg/png/webp)

UI rules:
- Profile page loads from `/users/me`.
- Logo upload validates file size/type client-side before sending.
- Update profile form supports nullable optional fields safely.

## 4) Clients

Endpoints:
- `GET /api/clients?search=&page=&pageSize=`
- `GET /api/clients/{id}`
- `POST /api/clients`
- `PUT /api/clients/{id}`
- `DELETE /api/clients/{id}`

UI/data rules:
- Support debounced search.
- Persist table pagination state (`page`, `pageSize`) in URL query params.
- On create/update/delete, invalidate/refetch client list and relevant details.

## 5) Jobs

Endpoints:
- `GET /api/jobs?status=&clientId=&search=&page=&pageSize=`
- `GET /api/jobs/{id}`
- `POST /api/jobs`
- `PUT /api/jobs/{id}`
- `PATCH /api/jobs/{id}/status`
- `DELETE /api/jobs/{id}`

UI/data rules:
- Status updates should be optimistic with rollback on failure.
- Keep job details synchronized with client and line item state.

## 6) Line Items

Endpoints:
- `POST /api/jobs/{jobId}/line-items`
- `PUT /api/line-items/{id}`
- `DELETE /api/line-items/{id}`

Rules:
- Recalculate and refresh quote/invoice totals after line-item mutation.
- Handle decimal/currency fields precisely (no floating-point display drift in UI formatting).

## 7) Quotes

Endpoints:
- `GET /api/quotes?jobId=&status=&page=&pageSize=`
- `GET /api/quotes/{id}`
- `POST /api/jobs/{jobId}/quotes`
- `PUT /api/quotes/{id}`
- `GET /api/quotes/{id}/pdf`
- `POST /api/quotes/{id}/send`

Rules:
- `pdf` endpoint should trigger secure file download/open (blob handling).
- `send` should provide success/failure feedback and disable duplicate submit while in-flight.

## 8) Invoices

Endpoints:
- `GET /api/invoices?jobId=&status=&page=&pageSize=`
- `GET /api/invoices/{id}`
- `POST /api/jobs/{jobId}/invoices`
- `PUT /api/invoices/{id}`
- `PATCH /api/invoices/{id}/paid`
- `GET /api/invoices/{id}/pdf`
- `POST /api/invoices/{id}/send`

Rules:
- Payment status transitions must update UI badges and dashboard aggregates.
- PDF and send behavior mirrors quote flows.

## 9) Dashboard & Billing

Endpoints:
- `GET /api/dashboard`
- `POST /api/billing/checkout`
- `POST /api/billing/portal`

Rules:
- Dashboard should load with resilient partial rendering (show available cards even if one widget fails).
- Billing checkout/portal responses include Stripe session URLs; redirect safely using returned URL.

## 10) Error handling contract

Normalize all API errors into shape:
```ts
type ApiError = {
  status: number;
  code?: string;
  title?: string;
  message: string;
  errors?: Record<string, string[]>;
  traceId?: string;
};
```

Status handling baseline:
- `400`: validation/message, show field-level errors when available.
- `401`: attempt refresh once; if still unauthorized, force sign-in.
- `403`: show permission/ownership error.
- `404`: show missing resource state.
- `409`: show conflict message (e.g., duplicate quote/invoice).
- `429`: show “too many requests” with retry guidance.
- `>=500`: generic server error + retry action.

## 11) Caching/state strategy

- Use a query cache library (or equivalent) with domain keys per entity/list params.
- Invalidate related keys after mutations.
- Avoid stale auth headers by always reading token from a single source-of-truth session store.

## 12) Security and correctness guardrails

- Never trust client-side ownership checks; rely on backend 403/404 and handle gracefully.
- Sanitize and encode user-generated fields in UI rendering.
- Never log access/refresh tokens.
- Use strict TypeScript types (no `any` in API layer).

## 13) Deliverables expected from you (frontend agent)

1. Typed API modules covering every endpoint above.
2. Auth/session manager with refresh lock and retry flow.
3. UI integration for profile, clients, jobs, quotes, invoices, dashboard, and billing actions.
4. Upload/download handling (`multipart` and `blob`).
5. Error handling system with reusable components/toasts.
6. A concise integration doc mapping screens to endpoints.

## 14) Verification checklist (must pass)

- Register → Login → Authenticated navigation works.
- Token expiration simulation triggers refresh and request replay.
- Parallel protected requests during token expiry perform one refresh only.
- CRUD flows for clients/jobs/line-items/quotes/invoices work end-to-end.
- Quote/invoice PDF downloads function correctly.
- Quote/invoice send actions provide clear success/failure UX.
- Dashboard loads and updates after key mutations.
- Billing checkout/portal redirect correctly.
- Validation, forbidden, conflict, and rate-limit errors are surfaced clearly.

If any endpoint response differs from expectation, trust actual backend payloads at runtime and update shared API typings accordingly, while keeping compatibility notes in the integration doc.