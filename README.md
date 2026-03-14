![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet)
![License](https://img.shields.io/badge/license-MIT-green)
![Status](https://img.shields.io/badge/status-active-success)



# 🔨 AuctionHub Backend API

A modern and secure **auction management REST API** built with **ASP.NET (.NET 10)**.
This backend powers the AuctionHub platform and provides features such as secure authentication, auction lifecycle management, and a real-time bidding system.

The API is fully documented using **Swagger (OpenAPI 3.0)** and is designed to work with the **AuctionHub React frontend**.

---

# 🖼️ API Documentation

The API includes full interactive documentation using **Swagger UI**.

Below is a preview of the Swagger interface.

![Swagger UI Screenshot](docs/swagger-ui.png)

After running the project you can access Swagger at:

http://localhost:5226/swagger

---

# 🌟 Key Features

## 👤 User Management

Secure authentication system including:

* User registration
* User login
* JWT token authentication
* BCrypt password hashing

Additional capabilities:

* Users can update their password (with verification)
* Admins can deactivate user accounts

---

## ⚖️ Auction System

AuctionHub supports a full auction lifecycle.

Features include:

* Create auctions
* Update auctions
* View active auctions
* View auction details

Smart bidding rules:

* Bids only allowed on **active auctions**
* New bids must exceed the **current highest bid**
* Automatic auction status tracking

Auction statuses:

* **Live** – currently active
* **Finished** – auction ended
* **Disabled** – disabled by admin

---

## 💰 Bid Management

Users can participate in auctions by placing bids.

Rules:

* Bids must be higher than the current highest bid
* Users may cancel their **latest bid** while the auction is active

---

## 🛡️ Admin Operations

Administrative features include:

* Disable auctions
* View all users
* View all auctions regardless of status

Role-based access control (RBAC):

* `User`
* `Admin`

Protected endpoints require **JWT authorization**.

---

# 💻 Tech Stack

Backend Framework

* ASP.NET Core (.NET 10)

Database

* Entity Framework Core

Authentication

* JWT (JSON Web Tokens)

Security

* BCrypt.Net password hashing

Architecture

* Layered Architecture
* Repository Pattern
* Service Layer

API Documentation

* Swagger / OpenAPI 3.0

---

# 🏗️ Architecture

The backend follows a **layered architecture** to ensure separation of concerns.

Controller Layer
Handles HTTP requests and responses.

Service Layer
Contains the main business logic such as auction rules and bid validation.

Repository Layer
Handles database access through Entity Framework Core.

Database Layer
Stores users, auctions, and bids.

Request flow:

Client
↓
Controller
↓
Service Layer
↓
Repository Layer
↓
Database

---

# 📁 Project Structure

```text
AuctionHub_backend
│
├── Controllers/
│   ├── AuctionController.cs
│   └── UserController.cs
│
├── Core/
│   │
│   ├── Interfaces/
│   │   ├── IAuctionService.cs
│   │   ├── ITokenService.cs
│   │   └── IUserService.cs
│   │
│   └── Services/
│       ├── AuctionService.cs
│       ├── TokenService.cs
│       └── UserService.cs
│
├── Data/
│   │
│   ├── Dtos/
│   │   ├── AuctionCreateDto.cs
│   │   ├── AuctionDetailDto.cs
│   │   ├── AuctionListDto.cs
│   │   ├── AuctionUpdateDto.cs
│   │   ├── AuthResponseDto.cs
│   │   ├── BidCreateDto.cs
│   │   ├── BidDto.cs
│   │   ├── UpdatePasswordDto.cs
│   │   ├── UserLoginDto.cs
│   │   ├── UserRegisterDto.cs
│   │   └── UserResponseDto.cs
│   │
│   ├── Entities/
│   │   ├── Auction.cs
│   │   ├── Bid.cs
│   │   └── User.cs
│   │
│   ├── Interfaces/
│   │   ├── IAuctionRepo.cs
│   │   └── IUserRepo.cs
│   │
│   ├── Repos/
│   │   ├── AuctionRepo.cs
│   │   └── UserRepo.cs
│   │
│   ├── Migrations/
│   │
│   └── AuctionDbContext.cs
│
├── appsettings.json
├── Program.cs
└── README.md
```

---

# ⚙️ Configuration

Application configuration is stored in:

appsettings.json

Example configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "YOUR_DATABASE_CONNECTION"
  },

  "Jwt": {
    "Key": "STORED_IN_USER_SECRETS",
    "Issuer": "AuctionHub_backend",
    "Audience": "AuctionHub_frontend",
    "ExpiresMinutes": 60
  }
}
```

For security reasons, the **JWT key should be stored using .NET User Secrets or environment variables**.

---



## 🚀 Quick Start

1. **Clone the Repository**
   ```bash
   git clone [https://github.com/Qian1507/AuctionHub_backend.git](https://github.com/Qian1507/AuctionHub_backend.git)
   cd AuctionHub_backend
---

## 2 Configure Database

Edit:

appsettings.json

Add your database connection string.

Example (SQL Server):

```
Server=localhost;Database=AuctionHub;Trusted_Connection=True;
```

---

## 3 Run Database Migration

```
dotnet ef database update
```

---

## 4 Run Application

```
dotnet run
```

Backend runs at:

```
http://localhost:5226
```

Swagger documentation:

```
http://localhost:5226/swagger
```

---

# 🔐 Authentication

The API uses **JWT authentication**.

Authentication flow:

1. User registers an account
2. Password is hashed using BCrypt
3. User logs in
4. Server returns a JWT token
5. Client sends the token in request headers

Example header:

```
Authorization: Bearer <token>
```

Protected endpoints require a valid token.

---

# 🌐 CORS

CORS is configured to allow requests from the AuctionHub frontend.

Development frontend URL:

```
http://localhost:5173
```

---

# 🔗 Related Project

AuctionHub uses a separate frontend repository.

Frontend:

AuctionHub_frontend

Built with:

* React
* TypeScript
* Vite

Repository:

https://github.com/Qian1507/AuctionHub_frontend

---

# 📄 License

This project is licensed under the MIT License.
