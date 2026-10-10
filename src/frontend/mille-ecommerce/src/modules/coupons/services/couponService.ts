import type { ApiResponse } from '@/shared/types/api'

import apiClient from '@/shared/services/axios'
import type {
  CouponResponse,
  CouponsResponse,
  CreateCouponRequest,
  CreateCouponResponse,
  DeleteCouponResponse,
  UpdateCouponRequest,
  UpdateCouponResponse,
  ValidateCouponRequest,
  ValidateCouponResponse,
} from '../types/coupon'
import type { CouponPaginationRequest } from '@/shared/types/pagination'

const BASE_URL = '/api/Coupons'

export const couponService = {
  async createCoupon(data: CreateCouponRequest): Promise<ApiResponse<CreateCouponResponse>> {
    const res = await apiClient.post<ApiResponse<CreateCouponResponse>>(`${BASE_URL}`, data)
    return res.data
  },

  async updateCoupon(
    id: number,
    data: UpdateCouponRequest,
  ): Promise<ApiResponse<UpdateCouponResponse>> {
    const res = await apiClient.put<ApiResponse<UpdateCouponResponse>>(`${BASE_URL}/${id}`, data)
    return res.data
  },

  async deleteCoupon(id: number): Promise<ApiResponse<DeleteCouponResponse>> {
    const res = await apiClient.delete<ApiResponse<DeleteCouponResponse>>(`${BASE_URL}/${id}`)
    return res.data
  },

  async getCoupon(id: number): Promise<ApiResponse<CouponResponse>> {
    const res = await apiClient.get<ApiResponse<CouponResponse>>(`${BASE_URL}/${id}`)
    return res.data
  },

  async getCoupons(params: CouponPaginationRequest): Promise<ApiResponse<CouponsResponse>> {
    const res = await apiClient.get<ApiResponse<CouponsResponse>>(`${BASE_URL}`, {
      params: params,
    })
    return res.data
  },

  async validateCoupon(data: ValidateCouponRequest): Promise<ApiResponse<ValidateCouponResponse>> {
    const res = await apiClient.post<ApiResponse<ValidateCouponResponse>>(
      `${BASE_URL}/validate`,
      data,
    )
    return res.data
  },
}
