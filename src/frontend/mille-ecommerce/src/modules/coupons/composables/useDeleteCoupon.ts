import { ref } from 'vue'
import { couponService } from '../services/couponService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'
import type { DeleteCouponResponse } from '../types/coupon'

export function useDeleteCoupon() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function deleteCoupon(id: number): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      await couponService.deleteCoupon(id)
      return true
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<DeleteCouponResponse>>(error)) {
        errorMessage.value =
          error.response?.data.message ?? `Failed to delete coupon with ID: ${id}.`
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

  return { isLoading, errorMessage, errors, deleteCoupon }
}
