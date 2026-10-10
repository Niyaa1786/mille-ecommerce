import z from 'zod'

export type ProductStatus = 'Active' | 'OutOfStock' | 'Contact' | 'Discontinued'

export const PRODUCT_STATUS_LABELS: Record<ProductStatus, string> = {
  Active: 'Active',
  OutOfStock: 'Out of stock',
  Contact: 'Contact',
  Discontinued: 'Discontinued',
}

export const variantSchema = z.object({
  sku: z.string().nonempty('SKU is required').max(100, 'SKU must not exceed 100 characters'),
  price: z.number().gt(0, 'Price must be greater than 0'),
  stock: z.number().gte(0, 'Stock cannot be negative'),
  size: z.string().max(20, 'Size must not exceed 20 characters').optional(),
  color: z.string().max(50, 'Color must not exceed 50 characters').optional(),
})

export const createProductSchema = z.object({
  name: z.string().nonempty('Product name is required').max(200, 'Name too long'),
  description: z.string().optional(),
  categoryId: z.number().gt(0, 'Please select a category'),
  variants: z.array(variantSchema).min(1, 'At least one variant is required'),
  images: z.array(z.file()).min(1, 'Required at least 1 image'),
  status: z.enum(['Active', 'OutOfStock', 'Contact', 'Discontinued'], {
    message: 'Invalid status',
  }),
})

export const updateVariantSchema = variantSchema.extend({
  id: z.string().optional(),
})

export const updateProductSchema = createProductSchema.omit({ images: true }).extend({
  variants: z.array(updateVariantSchema).min(1, 'At least one variant is required'),
})

export const updateProductImagesSchema = z.object({
  images: z.array(z.file()).min(1, 'Required at least 1 image'),
})

export type VariantRequest = z.infer<typeof variantSchema>
export type UpdateVariantRequest = z.infer<typeof updateVariantSchema>
export type CreateProductRequest = z.infer<typeof createProductSchema>
export type UpdateProductRequest = z.infer<typeof updateProductSchema>
export type UpdateProductImagesRequest = z.infer<typeof updateProductImagesSchema>

export type VariantResponse = {
  id: string
  sku: string
  size: string
  color: string
  price: number
  stock: number
  createdAt: string
  updateAt: string
}

export type ImageResponse = {
  id: string
  publicId: string
  imageUrl: string
  isThumbnail: boolean
}

export type CreateProductResponse = {
  id: string
  name: string
  categoryId: number
  description: string
  status: ProductStatus
  variants: VariantResponse[]
  images: ImageResponse[]
}

export type UpdateProductResponse = Omit<CreateProductResponse, 'images'>

export type UpdateProductImagesResponse = {
  productId: string
  images: ImageResponse[]
}

export type ProductsResponse = {
  items: ProductList[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
export type ProductResponse = {
  id: string
  name: string
  description: string
  categoryId: number
  categoryName: string
  status: ProductStatus
  createdAt: string
  updatedAt: string
  variants: VariantResponse[]
  images: ImageResponse[]
}

export type ProductList = {
  id: string
  name: string
  description: string
  categoryId: number
  categoryName: string
  status: ProductStatus
  minPrice: number
  thumbnailUrl: string
  createdAt: string
}

export type Product = ProductResponse
