import type { OrderStatus } from '@/modules/orders/types/order'
import type { ProductStatus } from '@/modules/products/types/product'

export type PaginationMetaResponse = {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export type PaginationMetaRequest = {
  page: number
  pageSize: number
  includeDeleted: boolean
  keyword?: string
}

export type ProductPaginationMetaRequest = PaginationMetaRequest & {
  categoryId?: number
  status?: ProductStatus
}

export type OrderPaginationRequest = {
  page: number
  pageSize: number
  status?: OrderStatus
  keyword?: string
}

export type CouponPaginationRequest = {
  page: number
  pageSize: number
  keyword?: string
  isActive?: boolean
}

export type ReviewPaginationRequest = {
  page: number
  pageSize: number
}
