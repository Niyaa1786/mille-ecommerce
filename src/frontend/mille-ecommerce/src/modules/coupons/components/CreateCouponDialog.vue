<script setup lang="ts">
import { ref } from 'vue'
import { useForm } from '@tanstack/vue-form'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useCreateCoupon } from '@/modules/coupons/composables/useCreateCoupon'
import {
  createCouponSchema,
  DISCOUNT_TYPE_LABELS,
  type CreateCouponRequest,
  type DiscountType,
} from '@/modules/coupons/types/coupon'

const emit = defineEmits<{
  (e: 'success'): void
}>()

const open = ref(false)
const { isLoading, errorMessage, errors, createCoupon } = useCreateCoupon()

const DISCOUNT_TYPE_OPTIONS: DiscountType[] = ['Percentage', 'FixedAmount']

const form = useForm({
  defaultValues: {
    code: '',
    description: '',
    discountType: 'Percentage',
    discountValue: 0,
    minOrderAmount: undefined,
    maxDiscountAmount: undefined,
    quantity: undefined,
    startDate: '',
    endDate: '',
  } as CreateCouponRequest,
  validators: {
    onSubmit: createCouponSchema,
  },
  onSubmit: async ({ value }) => {
    const success = await createCoupon({
      ...value,
      description: value.description || undefined,
      startDate: value.startDate || undefined,
      endDate: value.endDate || undefined,
    })
    if (success) {
      form.reset()
      open.value = false
      emit('success')
    }
  },
})

function handleOpenChange(value: boolean) {
  open.value = value
  if (value) form.reset()
}
</script>

<template>
  <Dialog :open="open" @update:open="handleOpenChange">
    <DialogTrigger as-child>
      <Button>Create Coupon</Button>
    </DialogTrigger>

    <DialogContent class="sm:max-w-lg max-h-[90vh] overflow-y-auto">
      <DialogHeader>
        <DialogTitle>Create Coupon</DialogTitle>
        <DialogDescription>Add a new discount coupon.</DialogDescription>
      </DialogHeader>

      <form class="grid grid-cols-2 gap-4" @submit.prevent="form.handleSubmit">
        <form.Field name="code">
          <template #default="{ field, state }">
            <div class="space-y-1 col-span-2">
              <Label :for="field.name">Code</Label>
              <Input
                :id="field.name"
                :name="field.name"
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(String(v))"
                @blur="field.handleBlur"
                placeholder="SUMMER2026"
              />
              <p v-if="state.meta.errors.length" class="text-sm text-destructive">
                {{ typeof state.meta.errors[0] === 'object' ? state.meta.errors[0]?.message : state.meta.errors[0] }}
              </p>
            </div>
          </template>
        </form.Field>
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

        <p v-if="errorMessage" class="col-span-2 text-sm text-destructive">{{ errorMessage }}</p>
        <ul v-if="errors.length" class="col-span-2 list-disc pl-4 text-sm text-destructive">
          <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
        </ul>

        <DialogFooter class="col-span-2">
          <Button type="button" variant="outline" :disabled="isLoading" @click="open = false"> Cancel </Button>
          <form.Subscribe>
            <template #default="{ canSubmit }">
              <Button type="submit" :disabled="!canSubmit || isLoading">
                {{ isLoading ? 'Creating...' : 'Create' }}
              </Button>
            </template>
          </form.Subscribe>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>
