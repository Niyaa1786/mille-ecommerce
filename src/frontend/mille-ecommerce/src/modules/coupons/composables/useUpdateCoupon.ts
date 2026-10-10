import type { ApiResponse } from '@/shared/types/api'
import { couponService } from '../services/couponService'
import type { UpdateCouponRequest, UpdateCouponResponse } from '../types/coupon'
import axios from 'axios'
import { ref } from 'vue'

export function useUpdateCoupon() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function updateCoupon(id: number, data: UpdateCouponRequest): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      await couponService.updateCoupon(id, data)
      return true
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<UpdateCouponResponse>>(error)) {
        errorMessage.value =
          error.response?.data.message ?? `Failed to update coupon with ID:${id}.`
        const errorList = error.response?.data.errors
        if (errorList && typeof errorList === 'object') {
          errors.value = Object.values(errorList).flat()
        }
      }
      return false
    } finally {
      isLoading.value = false
    }
  }

  return { isLoading, errorMessage, errors, updateCoupon }
}
