import { z } from 'zod'

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipping' | 'Completed' | 'Cancelled'

export const ORDER_STATUS_LABELS: Record<OrderStatus, string> = {
  Pending: 'Pending',
  Confirmed: 'Confirmed',
  Shipping: 'Shipping',
  Completed: 'Completed',
  Cancelled: 'Cancelled',
}

export type PaymentMethod = 'COD' | 'VNPay' | 'Stripe'

export const PAYMENT_METHOD_LABELS: Record<PaymentMethod, string> = {
  COD: 'COD',
  VNPay: 'VNPay',
  Stripe: 'Stripe',
}

export type PaymentStatus = 'Pending' | 'Success' | 'Failed' | 'Refunded'

export const PAYMENT_STATUS_LABELS: Record<PaymentStatus, string> = {
  Pending: 'Pending',
  Success: 'Success',
  Failed: 'Failed',
  Refunded: 'Refunded',
}

export const createOrderSchema = z.object({
  receiverName: z
    .string()
    .nonempty('Receiver name is required')
    .max(100, 'Receiver name must not exceed 100 characters'),
  receiverPhone: z
    .string()
    .nonempty('Receiver phone is required')
    .max(20, 'Receiver phone must not exceed 20 characters'),
  shippingAddress: z
    .string()
    .nonempty('Shipping address is required')
    .max(500, 'Shipping address must not exceed 500 characters'),
  paymentMethod: z.enum(['COD', 'VNPay', 'Stripe'], {
    message: 'Invalid payment method',
  }),
  couponCode: z.string().max(50, 'Coupon code must not exceed 50 characters').optional(),
})

export const updateOrderStatusSchema = z.object({
  newStatus: z.enum(['Pending', 'Confirmed', 'Shipping', 'Completed', 'Cancelled'], {
    message: 'Invalid status',
  }),
  note: z.string().max(500, 'Note must not exceed 500 characters').optional(),
})

export type CreateOrderRequest = z.infer<typeof createOrderSchema>
export type UpdateOrderStatusRequest = z.infer<typeof updateOrderStatusSchema>

export type CreateOrderResponse = {
  orderId: string
  totalAmount: number
  paymentMethod: PaymentMethod
}

export type UpdateOrderStatusResponse = {
  message: string
}

export type OrdersResponse = {
  orders: OrderSummaryDto[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export type GetOrderResponse = {
  id: string
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  totalAmount: number
  discountAmount: number
  couponCode: string | null
  status: string
  createdAt: string
  updatedAt: string
  items: OrderItemDto[]
  statusHistories: OrderStatusHistoryDto[]
  payment: PaymentDto | null
}

export type CancelOrderResponse = {
  message: string
}

export type ConfirmPaymentResponse = {
  status: string
  paidAt: string | null
  message: string
}

export type OrderSummaryDto = {
  id: string
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  totalAmount: number
  status: string
  paymentMethod: string
  paymentStatus: string
  createdAt: string
}

export type OrderItemDto = {
  id: number
  productVariantId: string
  productName: string
  sku: string
  quantity: number
  unitPrice: number
  subtotal: number
}

export type OrderStatusHistoryDto = {
  id: number
  status: string
  note: string | null
  createdAt: string
}

export type PaymentDto = {
  id: string
  method: string
  amount: number
  transactionId: string | null
  gatewayResponse: string | null
  status: string
  paidAt: string | null
  createdAt: string
}
