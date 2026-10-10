import { ref } from 'vue'
import { type ReviewsResponse } from '../types/review'
import type { PaginationMetaResponse, ReviewPaginationRequest } from '@/shared/types/pagination'
import { reviewService } from '../services/reviewService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'

export function useGetReviewsByProduct() {
  const reviews = ref<ReviewsResponse['items']>([])
  const averageRating = ref<number>(0)
  const pagination = ref<PaginationMetaResponse>({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  })

  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function fetchReviews(productId: string, params: ReviewPaginationRequest) {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      const res = await reviewService.getReviewsByProduct(productId, params)
      reviews.value = res.data?.items ?? []
      if (res.data) {
        averageRating.value = res.data.averageRating
        pagination.value = {
          page: res.data.page,
          pageSize: res.data.pageSize,
          totalCount: res.data.totalCount,
          totalPages: res.data.totalPages,
        }
      }
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<ReviewsResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? 'Failed to load reviews'
        const errorList = error.response?.data.errors
        if (errorList && typeof errorList === 'object') {
          errors.value = Object.values(errorList).flat()
        }
      }
    } finally {
      isLoading.value = false
    }
  }

  return { reviews, averageRating, pagination, isLoading, errorMessage, errors, fetchReviews }
}
