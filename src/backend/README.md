# E-Commerce Backend API

Backend API cho hệ thống thương mại điện tử, xây dựng theo Clean Architecture.

## Tech Stack

- **Language/Framework:** C#, ASP.NET Core
- **ORM:** Entity Framework Core
- **Architecture:** Clean Architecture (không dùng MediatR)
- **Authentication:** JWT (Access Token + Refresh Token)
- **Validation:** FluentValidation
- **Logging:** Serilog
- **API Documentation:** Scalar
- **Utility/Extension Tool** Scrutor
- **Dự kiến mở rộng:** Redis (caching), Docker (containerization)

## Danh sách chức năng

Trạng thái: ✅ Done · 🚧 In Progress · ⬜ To Do

### 1. Authentication & User

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| Register | Đăng ký tài khoản mới | ✅ |
| Login | Đăng nhập, trả về JWT access token + refresh token | ✅ |
| Refresh Token | Cấp lại access token khi hết hạn | ✅ |
| Logout | Thu hồi refresh token | ✅ |
| Get Profile | Xem thông tin tài khoản cá nhân |✅ |
| Update Profile | Cập nhật thông tin cá nhân | ✅ |
| Manage Address | Thêm/sửa/xóa địa chỉ giao hàng | ✅ |

### 2. Category & Product

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| CRUD Category (Admin) | Quản lý danh mục sản phẩm | ✅ |
| CRUD Product (Admin) | Quản lý sản phẩm | ✅ |
| CRUD Product Variant (Admin) | Quản lý biến thể (size, màu, giá, tồn kho) | ✅ |
| Upload Product Image (Admin) | Thêm/xóa ảnh sản phẩm, đặt ảnh đại diện | ✅ |
| Get Product List | Danh sách sản phẩm (filter theo danh mục, phân trang) | ✅ |
| Get Product Detail | Chi tiết sản phẩm kèm variant, ảnh, đánh giá | ✅ |

### 3. Cart

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| Get Cart | Xem giỏ hàng hiện tại | ✅ |
| Add to Cart | Thêm sản phẩm vào giỏ | ✅ |
| Update Cart Item | Cập nhật số lượng | ✅ |
| Remove Cart Item | Xóa sản phẩm khỏi giỏ | ✅ |

### 4. Order

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| Checkout / Create Order | Tạo đơn hàng từ giỏ hàng | ✅ |
| Get Order List | Danh sách đơn hàng của user | ✅ |
| Get Order Detail | Chi tiết đơn hàng | ✅ |
| Cancel Order | Hủy đơn (khi còn ở trạng thái Pending) | ✅ |
| Update Order Status (Admin) | Chuyển trạng thái đơn hàng, lưu lịch sử | ✅ |

### 5. Payment (COD)

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| Create Payment Record | Tạo bản ghi thanh toán COD khi tạo đơn | ✅ |
| Confirm Payment (Admin) | Xác nhận đã thu tiền khi giao hàng thành công | ✅ |
| Get Payment Status | Xem trạng thái thanh toán của đơn hàng | ✅ |

### 6. Review

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| Create Review | Đánh giá sản phẩm (chỉ khi đơn hàng đã hoàn thành, mỗi order item 1 đánh giá) | ✅ |
| Get Reviews by Product | Danh sách đánh giá theo sản phẩm (phân trang, kèm điểm trung bình) | ✅ |

### 7. Coupon

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| CRUD Coupon (Admin) | Quản lý mã giảm giá (xóa mềm bằng cách vô hiệu hóa nếu mã đã được dùng) | ✅ |
| Validate Coupon | Kiểm tra mã hợp lệ (hạn dùng, số lượt, điều kiện đơn tối thiểu) trên giỏ hàng hiện tại | ✅ |
| Apply Coupon | Áp dụng mã giảm giá khi checkout (`couponCode` trong Create Order), hoàn lượt dùng khi hủy đơn | ✅ |

### 8. Payment nâng cao (mở rộng)

| Chức năng | Mô tả | Trạng thái |
|---|---|---|
| VNPay Integration | Tạo giao dịch, xử lý callback | ⬜ |
| Stripe Integration | Tạo giao dịch, xử lý webhook | ⬜ |
| Verify Transaction | Xác thực kết quả thanh toán từ gateway | ⬜ |

## Roadmap phát triển

```
1. Auth + User/Address              ✅ Done
2. Category + Product + Variant + Image  ✅ Done
3. Cart                             ✅ Done
4. Order                            ✅ Done
5. Payment (COD)                    ✅ Done
6. Review                          ✅ Done
7. Coupon                          ✅ Done
8. Payment nâng cao (VNPay/Stripe)
9. Redis caching (mở rộng)
10. Docker hóa (mở rộng)
```