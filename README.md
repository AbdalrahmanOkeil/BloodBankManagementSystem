# Blood Bank Management System (ASP.NET Core Web API)

**Online Blood Bank Management System** to connect **Donors**, **Hospitals**, and **Admin** for managing blood donations, requests, and stock in a secure and organized way.

---

## Features

* **Donor Management**
  * Donor registration
  * Eligibility check (every 90 days)
  * Track total donations and last donation date

* **Hospital Management**
  * Create blood requests
  * View personal requests

* **Admin Dashboard**
  * Approve/Reject donations
  * Manage blood stock
  * Approve/Reject blood requests
  * Role-based access control (Admin, Hospital, Donor)

* **Security**
  * JWT Authentication
  * Refresh tokens
  * Role-based authorization

* **API Design & Architecture**
  * Clean Architecture (Layers: API, Application, Domain, Infrastructure)
  * Repository & Service patterns
  * Dependency Injection
  * DTOs (Data Transfer Objects)
  * Async programming
  * Logging (ILogger)
  * Global Exception Handling
  * Standardized API Responses using **Result Pattern**
  * Swagger for API documentation

---

## Technologies & Tools

* **Backend:** ASP.NET Core, C#
* **Database:** SQL Server, Entity Framework Core
* **Authentication & Security:** JWT, Refresh Tokens, Role-Based Access Control (Admin, Hospital, Donor)
* **Architecture & Patterns:**
  * Clean Architecture
  * Repository Pattern
  * Service Pattern
  * Dependency Injection (DI)
  * Result Pattern for standardized responses
* **Testing & Documentation:** Swagger, Postman
* **Other Skills:**
  * Async/Await programming
  * Logging using `ILogger<T>`
  * Exception handling with Global Exception Middleware
  * DTOs for request/response mapping
  * Role-based authorization using `[Authorize(Roles = "...")]`
 
 ### Testing
 - Unit testing using NUnit
 - Tested service layer and core business rules
 - Ensured correctness of donation eligibility, stock validation, and workflows


---

## API Endpoints

### Auth
* **Register Donor:** `POST /api/auth/register-donor`
* **Register Hospital:** `POST /api/auth/register-hospital`
* **Login:** `POST /api/auth/login`
* **Refresh Token:** `GET /api/auth/refresh-token`
* **Revoke Token:** `POST /api/auth/revoke-token`

### Donors
* **Complete Profile Data:** `POST /api/donors/complete-profile` (Donor)
* **Get Donor Profile** `GET /api/donors/me` (Donor)
* **Get User Donations:** `GET /api/donors/me/donations` (Donor)

### Hospitals
* **Complete Profile Data:** `POST /api/hospital/complete-profile` (Hospital)
* **Get Hospital Profile:** `GET /api/hospital/me` (Hospital)
* **Update Hospital Profile:** `PUT /api/hospital/me` (Hospital)

### Donations
* **Create Donation:** `POST /api/donations` (Donor)
* **Approve Donation:** `PUT /api/donations/approve/{id}` (Admin)
* **Reject Donation:** `PUT /api/donations/reject/{id}` (Admin)

### Blood Requests
* **Create Request:** `POST /api/bloodrequests` (Hospital)
* **Get My Requests:** `GET /api/bloodrequests/me` (Hospital)
* **Approve Request:** `PUT /api/bloodrequests/{id}/approve` (Admin)
* **Reject Request:** `PUT /api/bloodrequests/{id}/reject` (Admin)

### Blood Stock
* **Get All Stocks:** `GET /api/bloodstock`
* **Get Stock by Blood Type:** `GET /api/bloodstock/{bloodTypeId}`
* **Increase Stock:** `PUT /api/bloodstock/{bloodTypeId}/increase` (Admin, Hospital)
* **Decrease Stock:** `PUT /api/bloodstock/{bloodTypeId}/decrease` (Admin, Hospital)

---

### Setup

 Clone the repository:

```bash
git clone https://github.com/AbdalrahmanOkeil/BloodBankManagementSystem.git
