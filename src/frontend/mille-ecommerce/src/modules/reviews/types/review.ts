import z from 'zod'

export const MIN_RATING = 1
export const MAX_RATING = 5

export const createReviewSchema = z.object({
  orderItemId: z.number().int().gt(0, 'Order item ID must be valid'),
  rating: z
    .number()
    .int()
    .min(MIN_RATING, 'Please select a rating')
    .max(MAX_RATING, `Rating must be between ${MIN_RATING} and ${MAX_RATING}`),
  comment: z.string().max(1000, 'Comment must not exceed 1000 characters').optional(),
})

export type CreateReviewRequest = z.infer<typeof createReviewSchema>

export type CreateReviewResponse = {
  id: number
  orderItemId: number
  rating: number
  comment: string | null
  createdAt: string
  message: string
}

export type Review = {
  id: number
  userFullName: string
  rating: number
  comment: string | null
  sku: string
  createdAt: string
}

export type ReviewsResponse = {
  averageRating: number
  items: Review[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
