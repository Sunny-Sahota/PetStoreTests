# PetStore API Test Automation (C#)

## Overview

This project demonstrates an automated API testing framework built in **C#** to validate CRUD operations against the Swagger PetStore API.

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

The framework currently covers:

### Create

* Create a new pet with dynamic test data

### Read

* Retrieve pet by ID
* Validate returned data

### Update

* Modify pet name
* Validate update using retry/polling

### Delete

* Delete pet
* Validate deletion behaviour (handles API inconsistencies)

---

## Retry Strategy

A reusable retry mechanism is implemented to handle:

* Eventual consistency
* Delayed API updates
* Transient failures

This ensures tests remain **stable and reliable**, especially when validating updates.

---

## Tech Stack

* **C# (.NET 8)**
* **xUnit** – test framework
* **RestSharp** – HTTP client
* **Newtonsoft.Json** – serialization
* **FluentAssertions** – readable assertions

---

## Design Decisions

* Separation of concerns across layers improves maintainability
* Retry logic centralised to avoid duplication
* JSON handling abstracted for consistency and safety
* Tests written to reflect **business intent**

---

## Future Improvements

* Introduce logging/reporting
* Expand test coverage (negative scenarios, edge cases)

---

## Author 

Built as part of an SDET technical assessment, focusing on clean code, scalability, and real-world testing practices.
