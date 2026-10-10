import { ref } from 'vue'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'
import { couponService } from '../services/couponService'
import type { ValidateCouponRequest, ValidateCouponResponse } from '../types/coupon'

export function useValidateCoupon() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function validateCoupon(
    data: ValidateCouponRequest,
  ): Promise<ValidateCouponResponse | null> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      const res = await couponService.validateCoupon(data)
      return res.data
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<ValidateCouponResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? 'Failed to validate coupon.'
        const errorList = error.response?.data.errors
        if (errorList && typeof errorList === 'object') {
          errors.value = Object.values(errorList).flat()
        }
      }
      return null
    } finally {
      isLoading.value = false
    }
  }

  return { isLoading, errorMessage, errors, validateCoupon }
}
