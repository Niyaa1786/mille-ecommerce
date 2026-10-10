import apiClient from '@/shared/services/axios'
import type { ApiResponse } from '@/shared/types/api'
import type {
  CreateProductRequest,
  CreateProductResponse,
  ProductResponse,
  ProductsResponse,
  UpdateProductImagesResponse,
  UpdateProductRequest,
  UpdateProductResponse,
} from '../types/product'
import type { ProductPaginationMetaRequest } from '@/shared/types/pagination'

const BASE_URL = '/api/Products'

export const productService = {
  async createProduct(data: CreateProductRequest): Promise<ApiResponse<CreateProductResponse>> {
    const formData = new FormData()
    formData.append('name', data.name)
    formData.append('categoryId', data.categoryId.toString())
    formData.append('status', data.status)

    if (data.description) formData.append('description', data.description)

    data.variants.forEach((variant, idx) => {
      formData.append(`variants[${idx}].sku`, variant.sku)
      formData.append(`variants[${idx}].price`, variant.price.toString())
      formData.append(`variants[${idx}].stock`, variant.stock.toString())

      if (variant.size) formData.append(`variants[${idx}].size`, variant.size)
      if (variant.color) formData.append(`variants[${idx}].color`, variant.color)
    })

    data.images.forEach((img) => formData.append('images', img))

    const res = await apiClient.post<ApiResponse<CreateProductResponse>>(`${BASE_URL}`, formData)
    return res.data
  },

  async updateProduct(id: string, data: UpdateProductRequest): Promise<ApiResponse<UpdateProductResponse>> {
    const payload = {
      name: data.name,
      description: data.description || undefined,
      categoryId: data.categoryId,
      status: data.status,
      variants: data.variants.map((v) => ({
        id: v.id,
        sku: v.sku.trim(),
        price: v.price,
        stock: v.stock,
        size: v.size || undefined,
        color: v.color || undefined,
      })),
    }

    const res = await apiClient.put<ApiResponse<UpdateProductResponse>>(`${BASE_URL}/${id}`, payload)
    return res.data
  },

  async updateProductImages(id: string, images: File[]): Promise<ApiResponse<UpdateProductImagesResponse>> {
    const formData = new FormData()
    images.forEach((img) => formData.append('images', img))

    const res = await apiClient.put<ApiResponse<UpdateProductImagesResponse>>(`${BASE_URL}/${id}/images`, formData)
    return res.data
  },

  async deleteProduct(id: string): Promise<ApiResponse<null>> {
    const res = await apiClient.delete<ApiResponse<null>>(`${BASE_URL}/${id}`)
    return res.data
  },

  async getProduct(id: string): Promise<ApiResponse<ProductResponse>> {
    const res = await apiClient.get<ApiResponse<ProductResponse>>(`${BASE_URL}/${id}`)
    return res.data
  },

  async getProducts(params: ProductPaginationMetaRequest): Promise<ApiResponse<ProductsResponse>> {
    const res = await apiClient.get<ApiResponse<ProductsResponse>>(`${BASE_URL}`, {
      params: params,
    })
    return res.data
  },
}
