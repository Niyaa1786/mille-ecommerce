# E-Commerce Backend API

Backend API for an e-commerce system, built with Clean Architecture.

## Tech Stack

- **Language/Framework:** C#, ASP.NET Core
- **ORM:** Entity Framework Core
- **Architecture:** Clean Architecture (without MediatR)
- **Authentication:** JWT (Access Token + Refresh Token)
- **Validation:** FluentValidation
- **Logging:** Serilog
- **API Documentation:** Scalar
- **Utility/Extension Tool:** Scrutor
- **Containerization:** Docker
- **Planned extensions:** Redis (caching)

## Feature List

Status: ✅ Done · 🚧 In Progress · ⬜ To Do

### 1. Authentication & User

| Feature | Description | Status |
| --- | --- | --- |
| Register | Register a new account | ✅ |
| Login | Log in, returns JWT access token + refresh token | ✅ |
| Refresh Token | Issue a new access token when the old one expires | ✅ |
| Logout | Revoke the refresh token | ✅ |
| Get Profile | View personal account information | ✅ |
| Update Profile | Update personal information | ✅ |
| Manage Address | Add/edit/delete shipping addresses | ✅ |

### 2. Category & Product

| Feature | Description | Status |
| --- | --- | --- |
| CRUD Category (Admin) | Manage product categories | ✅ |
| CRUD Product (Admin) | Manage products | ✅ |
| CRUD Product Variant (Admin) | Manage variants (size, color, price, stock) | ✅ |
| Upload Product Image (Admin) | Add/remove product images, set the primary image | ✅ |
| Get Product List | Product list (filter by category, pagination) | ✅ |
| Get Product Detail | Product details including variants, images, reviews | ✅ |

### 3. Cart

| Feature | Description | Status |
| --- | --- | --- |
| Get Cart | View the current cart | ✅ |
| Add to Cart | Add a product to the cart | ✅ |
| Update Cart Item | Update quantity | ✅ |
| Remove Cart Item | Remove a product from the cart | ✅ |

### 4. Order

| Feature | Description | Status |
| --- | --- | --- |
| Checkout / Create Order | Create an order from the cart | ✅ |
| Get Order List | List of the user's orders | ✅ |
| Get Order Detail | Order details | ✅ |
| Cancel Order | Cancel an order (while still in Pending status) | ✅ |
| Update Order Status (Admin) | Change order status and keep a history | ✅ |

### 5. Payment (COD)

| Feature | Description | Status |
| --- | --- | --- |
| Create Payment Record | Create a COD payment record when the order is created | ✅ |
| Confirm Payment (Admin) | Confirm payment has been collected upon successful delivery | ✅ |
| Get Payment Status | View the payment status of an order | ✅ |

### 6. Review

| Feature | Description | Status |
| --- | --- | --- |
| Create Review | Review a product (only when the order is completed, one review per order item) | ✅ |
| Get Reviews by Product | List of reviews per product (paginated, with average rating) | ✅ |

### 7. Coupon

| Feature | Description | Status |
| --- | --- | --- |
| CRUD Coupon (Admin) | Manage discount codes (soft delete by deactivating if the code has already been used) | ✅ |
| Validate Coupon | Check whether a code is valid (expiry date, usage limit, minimum order value) against the current cart | ✅ |
| Apply Coupon | Apply a discount code at checkout (`couponCode` in Create Order), restore the usage count when an order is canceled | ✅ |

### 8. Advanced Payment (extension)

| Feature | Description | Status |
| --- | --- | --- |
| VNPay Integration | Create transactions, handle callbacks | ⬜ |
| Stripe Integration | Create transactions, handle webhooks | ⬜ |
| Verify Transaction | Verify payment results from the gateway | ⬜ |

### 9. Dockerization (extension)

| Feature | Description | Status |
| --- | --- | --- |
| Dockerfile | Multi-stage build (SDK image to build, ASP.NET image to run) | ✅ |
| Layer caching | Copy `.csproj` files and restore first to reuse the NuGet cache | ✅ |
| `.dockerignore` | Exclude `bin/`, `obj/`, `.vs/` from the build context | ✅ |

## Running with Docker

Build the image:

```bash
docker build -t mille-api .
```
Push to a registry:

```bash
docker tag mille-api <username>/mille-api:1.0
docker push <username>/mille-api:1.0
```

## Development Roadmap

```
1. Auth + User/Address                   ✅ Done
2. Category + Product + Variant + Image  ✅ Done
3. Cart                                  ✅ Done
4. Order                                 ✅ Done
5. Payment (COD)                         ✅ Done
6. Review                                ✅ Done
7. Coupon                                ✅ Done
8. Advanced Payment (VNPay/Stripe)
9. Redis caching (extension)
10. Dockerization (extension)            ✅ Done
```
