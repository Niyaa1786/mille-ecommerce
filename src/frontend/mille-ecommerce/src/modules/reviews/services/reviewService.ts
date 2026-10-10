import type { ApiResponse } from '@/shared/types/api'

import apiClient from '@/shared/services/axios'
import type { CreateReviewRequest, CreateReviewResponse, ReviewsResponse } from '../types/review'
import type { ReviewPaginationRequest } from '@/shared/types/pagination'

const BASE_URL = '/api/Reviews'

export const reviewService = {
  async createReview(data: CreateReviewRequest): Promise<ApiResponse<CreateReviewResponse>> {
    const res = await apiClient.post<ApiResponse<CreateReviewResponse>>(`${BASE_URL}`, data)
    return res.data
  },

  async getReviewsByProduct(productId: string, params: ReviewPaginationRequest): Promise<ApiResponse<ReviewsResponse>> {
    const res = await apiClient.get<ApiResponse<ReviewsResponse>>(`${BASE_URL}/product/${productId}`, {
      params: params,
    })
    return res.data
  },
}
