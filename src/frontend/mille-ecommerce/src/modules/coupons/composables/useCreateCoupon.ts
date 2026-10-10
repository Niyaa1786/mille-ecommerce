import { ref } from 'vue'
import type { CreateCouponRequest, CreateCouponResponse } from '../types/coupon'
import { couponService } from '../services/couponService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'

export function useCreateCoupon() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function createCoupon(data: CreateCouponRequest): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      await couponService.createCoupon(data)
      return true
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<CreateCouponResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? `Failed to create coupon.`
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

  return { isLoading, errorMessage, errors, createCoupon }
}
