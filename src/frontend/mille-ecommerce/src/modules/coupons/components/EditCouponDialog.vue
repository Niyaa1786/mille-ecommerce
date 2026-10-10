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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useUpdateCoupon } from '@/modules/coupons/composables/useUpdateCoupon'
import {
  updateCouponSchema,
  DISCOUNT_TYPE_LABELS,
  type Coupon,
  type DiscountType,
  type UpdateCouponRequest,
} from '@/modules/coupons/types/coupon'

const props = defineProps<{
  coupon: Coupon | null
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'success'): void
}>()

const { isLoading, errorMessage, errors, updateCoupon } = useUpdateCoupon()

const DISCOUNT_TYPE_OPTIONS: DiscountType[] = ['Percentage', 'Fixed']

const form = useForm({
  defaultValues: {
    description: '',
    discountType: 'Percentage',
    discountValue: 0,
    minOrderAmount: undefined,
    maxDiscountAmount: undefined,
    quantity: undefined,
    startDate: '',
    endDate: '',
    isActive: true,
  } as UpdateCouponRequest,
  validators: {
    onSubmit: updateCouponSchema,
  },
  onSubmit: async ({ value }) => {
    if (!props.coupon) return
    const success = await updateCoupon(props.coupon.id, {
      ...value,
      description: value.description || undefined,
      startDate: value.startDate || undefined,
      endDate: value.endDate || undefined,
    })
    if (success) {
      emit('update:open', false)
      emit('success')
    }
  },
})

watchEffect(() => {
  if (props.open && props.coupon) {
    form.setFieldValue('description', props.coupon.description ?? '')
    form.setFieldValue('discountType', props.coupon.discountType as DiscountType)
    form.setFieldValue('discountValue', props.coupon.discountValue)
    form.setFieldValue('minOrderAmount', props.coupon.minOrderAmount ?? undefined)
    form.setFieldValue('maxDiscountAmount', props.coupon.maxDiscountAmount ?? undefined)
    form.setFieldValue('quantity', props.coupon.quantity ?? undefined)
    form.setFieldValue('startDate', props.coupon.startDate?.slice(0, 16) ?? '')
    form.setFieldValue('endDate', props.coupon.endDate?.slice(0, 16) ?? '')
    form.setFieldValue('isActive', props.coupon.isActive)
  }
})

function handleOpenChange(value: boolean) {
  emit('update:open', value)
}
</script>

<template>
  <Dialog :open="open" @update:open="handleOpenChange">
    <DialogContent class="sm:max-w-lg max-h-[90vh] overflow-y-auto">
      <DialogHeader>
        <DialogTitle>Edit Coupon</DialogTitle>
        <DialogDescription>
          Update the details of coupon
          <span class="font-mono font-semibold text-foreground">{{ coupon?.code }}</span
          >.
        </DialogDescription>
      </DialogHeader>

      <form class="grid grid-cols-2 gap-4" @submit.prevent.stop="form.handleSubmit">
        <form.Field name="description">
          <template #default="{ field, state }">
            <div class="space-y-1 col-span-2">
              <Label :for="field.name">Description</Label>
              <Input
                :id="field.name"
                :name="field.name"
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(String(v))"
                @blur="field.handleBlur"
                placeholder="Optional description"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="discountType">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label>Discount type</Label>
              <Select
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(v as DiscountType)"
              >
                <SelectTrigger class="w-full">
                  <SelectValue placeholder="Select discount type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem v-for="t in DISCOUNT_TYPE_OPTIONS" :key="t" :value="t">
                    {{ DISCOUNT_TYPE_LABELS[t] }}
                  </SelectItem>
                </SelectContent>
              </Select>
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="discountValue">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Discount value</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="number"
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(v === '' ? 0 : Number(v))"
                @blur="field.handleBlur"
                placeholder="10"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="minOrderAmount">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Min order amount</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="number"
                :model-value="field.state.value ?? ''"
                @update:model-value="(v) => field.handleChange(v === '' ? undefined : Number(v))"
                @blur="field.handleBlur"
                placeholder="Optional"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="maxDiscountAmount">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Max discount amount</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="number"
                :model-value="field.state.value ?? ''"
                @update:model-value="(v) => field.handleChange(v === '' ? undefined : Number(v))"
                @blur="field.handleBlur"
                placeholder="Optional"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="quantity">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Quantity</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="number"
                :model-value="field.state.value ?? ''"
                @update:model-value="(v) => field.handleChange(v === '' ? undefined : Number(v))"
                @blur="field.handleBlur"
                placeholder="Unlimited"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="startDate">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">Start date</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="datetime-local"
                :model-value="field.state.value ?? ''"
                @update:model-value="(v) => field.handleChange(String(v))"
                @blur="field.handleBlur"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
        <form.Field name="endDate">
          <template #default="{ field, state }">
            <div class="space-y-1">
              <Label :for="field.name">End date</Label>
              <Input
                :id="field.name"
                :name="field.name"
                type="datetime-local"
                :model-value="field.state.value ?? ''"
                @update:model-value="(v) => field.handleChange(String(v))"
                @blur="field.handleBlur"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>

        <form.Field name="isActive">
          <template #default="{ field }">
            <label class="col-span-2 flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                :checked="field.state.value"
                @change="(e) => field.handleChange((e.target as HTMLInputElement).checked)"
              />
              Active
            </label>
          </template>
        </form.Field>

        <p v-if="errorMessage" class="col-span-2 text-sm text-destructive">{{ errorMessage }}</p>
        <ul v-if="errors.length" class="col-span-2 list-disc pl-4 text-sm text-destructive">
          <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
        </ul>

        <DialogFooter class="col-span-2">
          <Button type="button" variant="outline" :disabled="isLoading" @click="handleOpenChange(false)">
            Cancel
          </Button>
          <form.Subscribe>
            <template #default="{ canSubmit }">
              <Button type="submit" :disabled="!canSubmit || isLoading">
                {{ isLoading ? 'Saving...' : 'Save Changes' }}
              </Button>
            </template>
          </form.Subscribe>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
