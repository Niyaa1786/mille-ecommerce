import { ref } from 'vue'
import type { CreateReviewRequest, CreateReviewResponse } from '../types/review'
import { reviewService } from '../services/reviewService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'

export function useCreateReview() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function createReview(data: CreateReviewRequest): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      await reviewService.createReview(data)
      return true
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<CreateReviewResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? `Failed to submit review.`
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

  return { isLoading, errorMessage, errors, createReview }
}
