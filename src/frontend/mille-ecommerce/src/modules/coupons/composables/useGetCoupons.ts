import { ref } from 'vue'
import { type CouponsResponse } from '../types/coupon'
import type { CouponPaginationRequest, PaginationMetaResponse } from '@/shared/types/pagination'
import { couponService } from '../services/couponService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'

export function useGetCoupons() {
  const coupons = ref<CouponsResponse['items']>([])
  const pagination = ref<PaginationMetaResponse>({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  })

  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function fetchCoupons(params: CouponPaginationRequest) {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      const res = await couponService.getCoupons(params)
      coupons.value = res.data?.items ?? []
      if (res.data) {
        pagination.value = {
          page: res.data.page,
          pageSize: res.data.pageSize,
          totalCount: res.data.totalCount,
          totalPages: res.data.totalPages,
        }
      }
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<CouponsResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? 'Failed to load coupon list'
        const errorList = error.response?.data.errors
        if (errorList && typeof errorList === 'object') {
          errors.value = Object.values(errorList).flat()
        }
      }
    } finally {
      isLoading.value = false
    }
  }

  return { coupons, pagination, isLoading, errorMessage, errors, fetchCoupons }
}
