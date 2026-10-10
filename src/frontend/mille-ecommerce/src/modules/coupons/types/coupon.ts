import z from 'zod'

export type DiscountType = 'Percentage' | 'FixedAmount'

export const DISCOUNT_TYPE_LABELS: Record<DiscountType, string> = {
  Percentage: 'Percentage',
  FixedAmount: 'Fixed amount',
}

const discountTypeSchema = z.enum(['Percentage', 'FixedAmount'], {
  message: 'Invalid discount type',
})

export const createCouponSchema = z
  .object({
    code: z
      .string()
      .nonempty('Coupon code is required')
      .max(50, 'Coupon code must not exceed 50 characters')
      .regex(/^[A-Za-z0-9_-]+$/, "Coupon code can only contain letters, digits, '-' and '_'"),
    description: z.string().max(255, 'Description must not exceed 255 characters').optional(),
    discountType: discountTypeSchema,
    discountValue: z.number().gt(0, 'Discount value must be greater than zero'),
    minOrderAmount: z.number().gte(0, 'Minimum order amount cannot be negative').optional(),
    maxDiscountAmount: z.number().gt(0, 'Maximum discount amount must be greater than zero').optional(),
    quantity: z.number().int().gt(0, 'Quantity must be greater than zero').optional(),
    startDate: z.string().optional(),
    endDate: z.string().optional(),
  })
  .refine((v) => v.discountType !== 'Percentage' || v.discountValue <= 100, {
    message: 'Percentage discount cannot exceed 100',
    path: ['discountValue'],
  })
  .refine((v) => !v.startDate || !v.endDate || new Date(v.endDate) > new Date(v.startDate), {
    message: 'End date must be after start date',
    path: ['endDate'],
  })

export const updateCouponSchema = z
  .object({
    description: z.string().max(255, 'Description must not exceed 255 characters').optional(),
    discountType: discountTypeSchema,
    discountValue: z.number().gt(0, 'Discount value must be greater than zero'),
    minOrderAmount: z.number().gte(0, 'Minimum order amount cannot be negative').optional(),
    maxDiscountAmount: z.number().gt(0, 'Maximum discount amount must be greater than zero').optional(),
    quantity: z.number().int().gt(0, 'Quantity must be greater than zero').optional(),
    startDate: z.string().optional(),
    endDate: z.string().optional(),
    isActive: z.boolean(),
  })
  .refine((v) => v.discountType !== 'Percentage' || v.discountValue <= 100, {
    message: 'Percentage discount cannot exceed 100',
    path: ['discountValue'],
  })
  .refine((v) => !v.startDate || !v.endDate || new Date(v.endDate) > new Date(v.startDate), {
    message: 'End date must be after start date',
    path: ['endDate'],
  })

export const validateCouponSchema = z.object({
  code: z.string().nonempty('Coupon code is required').max(50, 'Coupon code must not exceed 50 characters'),
})

export type CreateCouponRequest = z.infer<typeof createCouponSchema>
export type UpdateCouponRequest = z.infer<typeof updateCouponSchema>
export type ValidateCouponRequest = z.infer<typeof validateCouponSchema>

export type Coupon = {
  id: number
  code: string
  description: string | null
  discountType: string
  discountValue: number
  minOrderAmount: number | null
  maxDiscountAmount: number | null
  quantity: number | null
  usedCount: number
  startDate: string | null
  endDate: string | null
  isActive: boolean
  createdAt: string
}

export type CreateCouponResponse = Coupon
export type UpdateCouponResponse = Coupon
export type CouponResponse = Coupon

export type CouponsResponse = {
  items: Coupon[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export type DeleteCouponResponse = {
  message: string
}

export type ValidateCouponResponse = {
  code: string
  discountType: string
  discountValue: number
  orderAmount: number
  discountAmount: number
  finalAmount: number
  message: string
}
