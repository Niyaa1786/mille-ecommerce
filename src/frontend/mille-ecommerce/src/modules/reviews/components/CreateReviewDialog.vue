<script setup lang="ts">
import { watchEffect } from 'vue'
import { useForm } from '@tanstack/vue-form'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import RatingStars from '@/modules/reviews/components/RatingStars.vue'
import { useCreateReview } from '@/modules/reviews/composables/useCreateReview'
import { createReviewSchema, type CreateReviewRequest } from '@/modules/reviews/types/review'
import type { OrderItemDto } from '@/modules/orders/types/order'

const props = defineProps<{
  orderItem: OrderItemDto | null
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'success'): void
}>()

const { isLoading, errorMessage, errors, createReview } = useCreateReview()

const form = useForm({
  defaultValues: {
    orderItemId: 0,
    rating: 0,
    comment: '',
  } as CreateReviewRequest,
  validators: {
    onSubmit: createReviewSchema,
  },
  onSubmit: async ({ value }) => {
    if (!props.orderItem) return
    const success = await createReview({
      ...value,
      orderItemId: props.orderItem.id,
      comment: value.comment || undefined,
    })
    if (success) {
      emit('update:open', false)
      emit('success')
    }
  },
})

watchEffect(() => {
  if (props.open && props.orderItem) {
    form.setFieldValue('orderItemId', props.orderItem.id)
    form.setFieldValue('rating', 0)
    form.setFieldValue('comment', '')
  }
})

function handleOpenChange(value: boolean) {
  emit('update:open', value)
}
</script>

<template>
  <Dialog :open="open" @update:open="handleOpenChange">
    <DialogContent class="sm:max-w-md">
      <DialogHeader>
        <DialogTitle>Write a Review</DialogTitle>
        <DialogDescription>
          Share your experience with
          <span class="font-semibold text-foreground">{{ orderItem?.productName }}</span>
          <span v-if="orderItem" class="font-mono text-xs"> ({{ orderItem.sku }})</span>.
        </DialogDescription>
      </DialogHeader>

      <form class="flex flex-col gap-4" @submit.prevent.stop="form.handleSubmit">
        <form.Field name="rating">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label>Rating</Label>
              <RatingStars :model-value="field.state.value" @update:model-value="(v) => field.handleChange(v)" />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>

        <form.Field name="comment">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Comment</Label>
              <Input
                :id="field.name"
                :name="field.name"
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(String(v))"
                @blur="field.handleBlur"
                placeholder="Optional comment"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>

        <p v-if="errorMessage" class="text-sm text-destructive">{{ errorMessage }}</p>
        <ul v-if="errors.length" class="list-disc pl-4 text-sm text-destructive">
          <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
        </ul>

        <DialogFooter>
          <Button type="button" variant="outline" :disabled="isLoading" @click="handleOpenChange(false)">
            Cancel
          </Button>
          <form.Subscribe>
            <template #default="{ canSubmit }">
              <Button type="submit" :disabled="!canSubmit || isLoading">
                {{ isLoading ? 'Submitting...' : 'Submit Review' }}
              </Button>
            </template>
          </form.Subscribe>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
