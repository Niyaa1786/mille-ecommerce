import { ref } from 'vue'
import type { UpdateProductImagesResponse } from '../types/product'
import { productService } from '../services/productService'
import axios from 'axios'
import type { ApiResponse } from '@/shared/types/api'

export function useUpdateProductImages() {
  const isLoading = ref<boolean>(false)
  const errorMessage = ref<string | null>(null)
  const errors = ref<string[]>([])

  async function updateProductImages(id: string, images: File[]): Promise<boolean> {
    isLoading.value = true
    errorMessage.value = null
    errors.value = []

    try {
      await productService.updateProductImages(id, images)
      return true
    } catch (error) {
      if (axios.isAxiosError<ApiResponse<UpdateProductImagesResponse>>(error)) {
        errorMessage.value = error.response?.data.message ?? `Failed to update images of product with ID: ${id}.`
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
  return { isLoading, errorMessage, errors, updateProductImages }
}
