import { ref } from 'vue'
import { couponService } from '../services/couponService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'
import type { CouponResponse } from '../types/coupon'

export function useGetCoupon() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  const coupon = ref<CouponResponse | null>(null)

  async function fetchCoupon(id: number) {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      coupon.value = (await couponService.getCoupon(id)).data
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<CouponResponse>>(error)) {
        errorMessage.value =
          error.response?.data.message ?? `Failed to load coupon with ID: ${id}, please try again.`
        const errorList = error.response?.data.errors
        if (errorList && typeof errorList === 'object') {
          errors.value = Object.values(errorList).flat()
        }
      }
    } finally {
      isLoading.value = false
    }
  }

  return { coupon, isLoading, errorMessage, errors, fetchCoupon }
}
