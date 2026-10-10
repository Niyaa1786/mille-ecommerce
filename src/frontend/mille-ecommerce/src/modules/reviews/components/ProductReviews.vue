<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { MessageSquareOff } from 'lucide-vue-next'

import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { Separator } from '@/components/ui/separator'

import RatingStars from '@/modules/reviews/components/RatingStars.vue'
import { useGetReviewsByProduct } from '@/modules/reviews/composables/useGetReviewsByProduct'
import { formatDate } from '@/shared/utils/format'

const props = defineProps<{
  productId: string
}>()

const { reviews, averageRating, pagination, isLoading, errorMessage, fetchReviews } = useGetReviewsByProduct()

const page = ref(1)
const pageSize = ref(10)

function loadReviews() {
  fetchReviews(props.productId, {
    page: page.value,
    pageSize: pageSize.value,
  })
}

function goToPage(next: number) {
  if (next < 1 || next > pagination.value.totalPages) return
  page.value = next
  loadReviews()
}

watch(
  () => props.productId,
  () => {
    page.value = 1
    loadReviews()
  },
)

onMounted(loadReviews)
</script>

<template>
  <div class="border-t pt-6 space-y-4">
    <div class="flex flex-wrap items-center justify-between gap-3">
      <h2 class="text-lg font-semibold">Customer reviews</h2>
      <div v-if="pagination.totalCount > 0" class="flex items-center gap-2 text-sm">
        <RatingStars :model-value="averageRating" readonly />
        <span class="font-medium">{{ averageRating.toFixed(1) }}</span>
        <span class="text-muted-foreground">({{ pagination.totalCount }} review(s))</span>
      </div>
    </div>

    <!-- Error -->
    <p v-if="errorMessage" class="text-sm text-destructive">{{ errorMessage }}</p>

    <!-- Loading -->
    <div v-if="isLoading" class="flex flex-col gap-4">
      <Skeleton v-for="n in 3" :key="n" class="h-20 w-full rounded-lg" />
    </div>

    <!-- Empty -->
    <div
      v-else-if="reviews.length === 0"
      class="flex flex-col items-center justify-center gap-2 rounded-lg border border-dashed py-12 text-center"
    >
      <MessageSquareOff class="h-8 w-8 text-muted-foreground" />
      <p class="text-sm text-muted-foreground">No reviews yet for this product.</p>
    </div>

    <!-- Data -->
    <div v-else class="flex flex-col gap-4">
      <template v-for="(review, index) in reviews" :key="review.id">
        <Separator v-if="index > 0" />
        <div class="space-y-1">
          <div class="flex flex-wrap items-center justify-between gap-2">
            <div class="flex items-center gap-2">
              <span class="text-sm font-medium">{{ review.userFullName }}</span>
              <RatingStars :model-value="review.rating" readonly />
            </div>
            <span class="text-xs text-muted-foreground">{{ formatDate(review.createdAt) }}</span>
          </div>
          <p class="font-mono text-xs text-muted-foreground">{{ review.sku }}</p>
          <p v-if="review.comment" class="whitespace-pre-line text-sm">{{ review.comment }}</p>
        </div>
      </template>

      <!-- Pagination -->
      <div v-if="pagination.totalPages > 1" class="mt-2 flex items-center justify-center gap-2">
        <Button
          variant="outline"
          size="sm"
          :disabled="pagination.page <= 1 || isLoading"
          @click="goToPage(pagination.page - 1)"
        >
          Previous
        </Button>
        <span class="text-sm text-muted-foreground"> Page {{ pagination.page }} / {{ pagination.totalPages }} </span>
        <Button
          variant="outline"
          size="sm"
          :disabled="pagination.page >= pagination.totalPages || isLoading"
          @click="goToPage(pagination.page + 1)"
        >
          Next
        </Button>
      </div>
    </div>
  </div>
</template>
