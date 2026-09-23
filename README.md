# PetStore API Test Automation

[![CI](https://img.shields.io/github/actions/workflow/status/Sunny-Sahota/PetStoreTests/dotnet-tests.yml?branch=master&label=CI)](https://github.com/Sunny-Sahota/PetStoreTests/actions/workflows/dotnet-tests.yml)
![Coverage](https://raw.githubusercontent.com/Sunny-Sahota/PetStoreTests/master/badges/coverage.svg)

A layered C# / xUnit framework that automates API testing against the [Swagger PetStore](https://petstore.swagger.io) demo API — covering CRUD, filtering, image upload, negative cases, and schema-driven edge cases with a deterministic local mock.

---

## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run

```bash
git clone https://github.com/Sunny-Sahota/PetStoreTests.git
cd PetStoreTests
dotnet restore
dotnet test
```

> **Important:** Tests hit the public PetStore demo API, so **an internet connection is required**. The live API is a shared service — see [Live API Dependency](#live-api-dependency).

### Configuration

The base URL defaults to `https://petstore.swagger.io/v2` and can be overridden without code changes:

* `appsettings.json` → set the `BaseUrl` key
* or an environment variable → `BaseUrl=https://petstore.swagger.io/v2`

```
dotnet test
```

---

## Live API Dependency

Tests run against the public PetStore demo API (https://petstore.swagger.io/v2). This is a **shared, externally hosted service**, so be aware:

* Results depend on the API being **available** — transient outages or slow responses can cause failures (retry logic helps).
* The API is shared: `GET /pet/findByStatus` may return pets created by other users, and the data set changes between runs.
* Deterministic negative/edge-case scenarios are covered locally via [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net); CRUD and positive scenarios run against the live API.

If a CI run fails with a network/timeout/empty-result error, check that petstore.swagger.io is up before assuming a test regression.

---

## Architecture

The framework is structured using a layered approach to ensure clear separation of concerns:

### Client Layer

* Responsible for raw HTTP communication using RestSharp
* No assertions or business logic
* Returns raw `RestResponse`

### Action Layer

* Converts API responses into domain models
* Performs basic validation (e.g. response success, deserialization)
* Acts as the bridge between API and domain logic

### Service Layer

* Contains business logic and state validation
* Handles retry/polling logic for eventual consistency scenarios
* Example: waiting for a pet name to update

### Test Layer

* Contains test scenarios and assertions
* Focuses on business validation rather than HTTP details

---

## Project Structure

```
PetStoreTests/
│
├── Actions/        # Business actions (API to domain)
├── Clients/        # Raw HTTP communication
├── Config/         # API configuration
├── Helpers/        # Test data creation
├── Models/         # Domain models
├── Services/       # Business logic / polling
├── Tests/          # Test scenarios
└── Utilities/      # Shared helpers (Retry, JSON, Assertions)
```

---

## Test Coverage

The framework covers a broad surface of the PetStore API:

| Area | What is tested |
| --- | --- |
| **CRUD** | Create, read, update (name + full schema), and delete a pet |
| **Filter** | `GET /pet/findByStatus` for `available`, `pending`, and `sold` |
| **Upload** | `POST /pet/{id}/uploadImage` with a fake (unvalidated) PNG payload |
| **Negative** | GET a nonexistent pet → `404` |
| **Edge Cases** | Empty, very long, special-character, and Unicode names |
| **Validation (mocked)** | Invalid body `400`, mismatched-ID update `404`, stateful delete, invalid status `400`, strict name-schema rejection `400` |

Tests are grouped with xUnit categories (`CRUD`, `Filter`, `Upload`, `Negative`, `EdgeCase`) so CI output and reports stay readable.

---

## Retry Strategy

Polls against the live shared API are wrapped in a reusable `RetryHelper` (built on a Polly v8 resilience pipeline with retry + circuit breaker) to handle:

* **Eventual consistency** — writes may take a moment to be visible to reads (`GET /pet/{id}`, `findByStatus`)
* **Delayed API updates** — a pet name change is verified via polling until the new value is returned
* **Transient errors** — request/response hiccups or a momentarily empty result set on a shared service

**Defaults:** the standard action-level retry is **5 attempts at 500 ms intervals**. The broader service-level waits (name update, status search visibility) use **10 attempts at 500 ms intervals**.

**Tradeoff:** polling trades a little runtime for stability against the shared public API. So the API cannot be relied on for exhaustive state and the strict negative/schema assertions live in the WireMock.Net suite instead. This allows deterministic and hermetic coverage for the cases that matter most.

---

## Tech Stack

[![.NET](https://img.shields.io/badge/.NET-8-blue)]()

* **C# (.NET 8)** – language / runtime
* **xUnit** – test framework (facts, theories, traits, fixtures)
* **RestSharp** – HTTP client
* **Newtonsoft.Json** – serialization
* **Polly** – retry/circuit-breaker resilience for live-API polling
* **FluentAssertions** – readable assertions
* **WireMock.Net** – hermetic mock server for deterministic negative/edge-case tests
* **Coverlet** – cross-platform code coverage (collected in CI)
* **JUnitXml.TestLogger** – emits JUnit-format results alongside TRX for CI test reports/artifacts
* **GitHub Actions** – CI pipeline (build, test, coverage gate, test reports, badges)

---

## Design Decisions

* Separation of concerns across layers improves maintainability
* Retry logic centralised to avoid duplication
* JSON handling abstracted for consistency and safety
* Base URL is configurable via `appsettings.json` or an environment variable
* Negative/schema tests are mocked with WireMock.Net so CI never depends on the live API's whims
* CI publishes both TRX and JUnit results — a readable test report via `dorny/test-reporter`, plus downloadable artifacts for offline debugging
* Tests written to reflect **business intent**
* HTTP traffic is captured per-test via a delegating `HttpMessageHandler` and streamed to xUnit's `ITestOutputHelper` — failed tests automatically carry the full request/response exchange in CI artifacts for offline debugging

---

## Future Improvements

* Cover additional API endpoints (e.g. `/store/inventory`, `/user`)

---

## Author

**Sunny Sahota**
SDET / Test Automation Engineer
[GitHub](https://github.com/Sunny-Sahota) · [LinkedIn](https://www.linkedin.com/in/sunny-sahota/)

Passionate about clean and maintainable test automation that tells a story.