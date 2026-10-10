<script setup lang="ts">
import { Star } from 'lucide-vue-next'
import { MAX_RATING } from '@/modules/reviews/types/review'

const props = defineProps<{
  modelValue: number
  readonly?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: number): void
}>()

function select(value: number) {
  if (props.readonly) return
  emit('update:modelValue', value)
}
</script>

<template>
  <div class="flex items-center gap-0.5">
    <button
      v-for="n in MAX_RATING"
      :key="n"
      type="button"
      class="transition-colors"
      :class="readonly ? 'cursor-default' : 'cursor-pointer'"
      :disabled="readonly"
      :aria-label="`${n} star${n > 1 ? 's' : ''}`"
      @click="select(n)"
    >
      <Star
        class="size-4"
        :class="n <= Math.round(modelValue) ? 'fill-current text-yellow-500' : 'text-muted-foreground'"
      />
    </button>
  </div>
</template>
