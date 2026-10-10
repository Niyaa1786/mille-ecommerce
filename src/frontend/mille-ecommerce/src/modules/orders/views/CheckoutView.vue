<template>
  <div class="mx-auto max-w-4xl">
    <h1 class="mb-6 text-2xl font-bold">Checkout</h1>

    <!-- Loading -->
    <div v-if="isLoadingCart" class="flex flex-col gap-4">
      <Skeleton class="h-40 w-full rounded-lg" />
      <Skeleton class="h-40 w-full rounded-lg" />
    </div>

    <!-- Empty cart guard -->
    <div
      v-else-if="isEmpty"
      class="flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed py-24 text-center"
    >
      <ShoppingCart class="h-12 w-12 text-muted-foreground" />
      <p class="font-medium">Your cart is empty</p>
      <p class="text-sm text-muted-foreground">Add some products before checking out.</p>
      <RouterLink to="/products">
        <Button class="mt-2">Continue shopping</Button>
      </RouterLink>
    </div>

    <div v-else class="grid grid-cols-1 gap-8 lg:grid-cols-3">
      <!-- Shipping & payment form -->
      <div class="lg:col-span-2">
        <Card>
          <CardHeader>
            <CardTitle>Shipping Information</CardTitle>
            <CardDescription>Where should we deliver your order?</CardDescription>
          </CardHeader>
          <CardContent>
            <!-- Saved addresses -->
            <div v-if="profile?.addresses.length" class="mb-6 space-y-2">
              <Label>Saved addresses</Label>
              <div class="flex flex-col gap-2">
                <button
                  v-for="address in profile.addresses"
                  :key="address.id"
                  type="button"
                  class="flex items-start justify-between gap-4 rounded-lg border p-3 text-left transition-colors"
                  :class="
                    selectedAddressId === address.id ? 'border-foreground bg-accent' : 'hover:border-foreground/50'
                  "
                  @click="applyAddress(address)"
                >
                  <div class="space-y-0.5">
                    <div class="flex items-center gap-2">
                      <span class="text-sm font-medium">{{ address.receiverName }}</span>
                      <Badge v-if="address.isDefault" variant="secondary">Default</Badge>
                    </div>
                    <p class="text-xs text-muted-foreground">{{ address.receiverPhone }}</p>
                    <p class="text-xs text-muted-foreground">{{ address.addressLine }}</p>
                  </div>
                  <Check v-if="selectedAddressId === address.id" class="mt-0.5 h-4 w-4 shrink-0 text-foreground" />
                </button>
              </div>
              <Separator class="mt-4" />
            </div>

            <form class="flex flex-col gap-4 pt-4" @submit.prevent.stop="form.handleSubmit">
              <form.Field name="receiverName">
                <template #default="{ field, state }">
                  <div class="space-y-1">
                    <Label :for="field.name">Receiver name</Label>
                    <Input
                      :id="field.name"
                      :name="field.name"
                      :model-value="field.state.value"
                      @update:model-value="(v) => field.handleChange(String(v))"
                      @blur="field.handleBlur"
                      placeholder="John Doe"
                    />
                    <p v-if="getErrorMessage(state.meta.errors)" class="text-sm text-destructive">
                      {{ getErrorMessage(state.meta.errors) }}
                    </p>
                  </div>
                </template>
              </form.Field>

              <form.Field name="receiverPhone">
                <template #default="{ field, state }">
                  <div class="space-y-1">
                    <Label :for="field.name">Receiver phone</Label>
                    <Input
                      :id="field.name"
                      :name="field.name"
                      type="tel"
                      :model-value="field.state.value"
                      @update:model-value="(v) => field.handleChange(String(v))"
                      @blur="field.handleBlur"
                      placeholder="0987654321"
                    />
                    <p v-if="getErrorMessage(state.meta.errors)" class="text-sm text-destructive">
                      {{ getErrorMessage(state.meta.errors) }}
                    </p>
                  </div>
                </template>
              </form.Field>

              <form.Field name="shippingAddress">
                <template #default="{ field, state }">
                  <div class="space-y-1">
                    <Label :for="field.name">Shipping address</Label>
                    <Input
                      :id="field.name"
                      :name="field.name"
                      :model-value="field.state.value"
                      @update:model-value="(v) => field.handleChange(String(v))"
                      @blur="field.handleBlur"
                      placeholder="Street, ward, district, city"
                    />
                    <p v-if="getErrorMessage(state.meta.errors)" class="text-sm text-destructive">
                      {{ getErrorMessage(state.meta.errors) }}
                    </p>
                  </div>
                </template>
              </form.Field>

              <form.Field name="paymentMethod">
                <template #default="{ field, state }">
                  <div class="space-y-1">
                    <Label>Payment method</Label>
                    <Select
                      :model-value="field.state.value"
                      @update:model-value="(v) => field.handleChange(v as PaymentMethod)"
                    >
                      <SelectTrigger class="w-full">
                        <SelectValue placeholder="Select payment method" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem v-for="m in PAYMENT_METHOD_OPTIONS" :key="m" :value="m">
                          {{ PAYMENT_METHOD_LABELS[m] }}
                        </SelectItem>
                      </SelectContent>
                    </Select>
                    <p v-if="getErrorMessage(state.meta.errors)" class="text-sm text-destructive">
                      {{ getErrorMessage(state.meta.errors) }}
                    </p>
                  </div>
                </template>
              </form.Field>

              <p v-if="createOrderError" class="text-sm text-destructive">
                {{ createOrderError }}
              </p>

              <form.Subscribe>
                <template #default="{ canSubmit }">
                  <Button type="submit" size="lg" class="w-full" :disabled="!canSubmit || isCreatingOrder">
                    {{ isCreatingOrder ? 'Placing order...' : 'Place order' }}
                  </Button>
                </template>
              </form.Subscribe>
            </form>
          </CardContent>
        </Card>
      </div>

      <!-- Order summary -->
      <div class="lg:col-span-1">
        <Card>
          <CardHeader>
            <CardTitle>Order Summary</CardTitle>
            <CardDescription>{{ totalItems }} item(s)</CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div class="flex max-h-72 flex-col gap-3 overflow-y-auto pr-1">
              <div v-for="item in items" :key="item.id" class="flex gap-3">
                <div class="h-14 w-14 shrink-0 overflow-hidden rounded-md bg-muted">
                  <img
                    v-if="item.thumbnailUrl"
                    :src="item.thumbnailUrl"
                    :alt="item.productName"
                    class="h-full w-full object-cover"
                  />
                  <div v-else class="flex h-full w-full items-center justify-center text-muted-foreground">
                    <ImageOff class="h-4 w-4" />
                  </div>
                </div>
                <div class="flex-1">
                  <p class="line-clamp-1 text-sm font-medium">{{ item.productName }}</p>
                  <p class="text-xs text-muted-foreground">
                    <span v-if="item.size || item.color">
                      {{ [item.size, item.color].filter(Boolean).join(' · ') }} ·
                    </span>
                    Qty {{ item.quantity }}
                  </p>
                </div>
                <span class="shrink-0 text-sm font-medium">{{ formatCurrency(item.subtotal) }}</span>
              </div>
            </div>

            <Separator />

            <!-- Coupon -->
            <div class="space-y-2">
              <Label for="coupon-code">Coupon code</Label>
              <div v-if="appliedCoupon" class="flex items-center justify-between rounded-md border p-2">
                <div class="flex items-center gap-2 text-sm">
                  <Ticket class="h-4 w-4 text-muted-foreground" />
                  <span class="font-mono font-medium">{{ appliedCoupon.code }}</span>
                </div>
                <Button type="button" variant="ghost" size="icon-sm" title="Remove coupon" @click="removeCoupon">
                  <X class="size-4" />
                </Button>
              </div>
              <div v-else class="flex items-center gap-2">
                <Input
                  id="coupon-code"
                  v-model="couponCode"
                  placeholder="Enter coupon code"
                  @keyup.enter="applyCoupon"
                />
                <Button
                  type="button"
                  variant="outline"
                  :disabled="!couponCode.trim() || isValidatingCoupon"
                  @click="applyCoupon"
                >
                  {{ isValidatingCoupon ? 'Applying...' : 'Apply' }}
                </Button>
              </div>
              <p v-if="couponError && !appliedCoupon" class="text-sm text-destructive">
                {{ couponError }}
              </p>
            </div>

            <Separator />

            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">Subtotal</span>
              <span>{{ formatCurrency(totalPrice) }}</span>
            </div>
            <div v-if="appliedCoupon" class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">Discount</span>
              <span>-{{ formatCurrency(appliedCoupon.discountAmount) }}</span>
            </div>
            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">Shipping</span>
              <span>Free</span>
            </div>
            <Separator />
            <div class="flex items-center justify-between font-semibold">
              <span>Total</span>
              <span>{{ formatCurrency(finalTotal) }}</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useForm } from '@tanstack/vue-form'
import { toast } from 'vue-sonner'
import { Check, ImageOff, ShoppingCart, Ticket, X } from 'lucide-vue-next'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'
import { Separator } from '@/components/ui/separator'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

import { useGetCart } from '@/modules/carts/composables/useGetCart'
import { useCreateOrder } from '../composables/useCreateOrder'
import { useGetProfile } from '@/modules/users/composables/useGetProfile'
import { useValidateCoupon } from '@/modules/coupons/composables/useValidateCoupon'
import { useCartStore } from '@/modules/carts/stores/cartStore'
import { createOrderSchema, PAYMENT_METHOD_LABELS, type CreateOrderRequest, type PaymentMethod } from '../types/order'
import type { AddressDto } from '@/modules/users/types/user'
import type { ValidateCouponResponse } from '@/modules/coupons/types/coupon'
import { formatCurrency } from '@/shared/utils/format'

const router = useRouter()
const cartStore = useCartStore()

const { items, totalPrice, totalItems, isEmpty, isLoading: isLoadingCart, fetchCart } = useGetCart()
const { profile, fetchProfile } = useGetProfile()
const { isLoading: isCreatingOrder, errorMessage: createOrderError, createOrder } = useCreateOrder()

const { isLoading: isValidatingCoupon, errorMessage: couponError, validateCoupon } = useValidateCoupon()

const couponCode = ref('')
const appliedCoupon = ref<ValidateCouponResponse | null>(null)

const finalTotal = computed(() => appliedCoupon.value?.finalAmount ?? totalPrice.value)

async function applyCoupon() {
  const code = couponCode.value.trim()
  if (!code) return
  const result = await validateCoupon({ code })
  if (result) {
    appliedCoupon.value = result
    toast.success(result.message)
  }
}

function removeCoupon() {
  appliedCoupon.value = null
  couponCode.value = ''
}

const PAYMENT_METHOD_OPTIONS: PaymentMethod[] = ['COD', 'VNPay', 'Stripe']
const selectedAddressId = ref<number | null>(null)

function getErrorMessage(errors: unknown[]): string {
  const first = errors[0]
  if (!first) return ''
  return typeof first === 'object' && first !== null && 'message' in first
    ? String((first as { message: unknown }).message)
    : String(first)
}

const form = useForm({
  defaultValues: {
    receiverName: '',
    receiverPhone: '',
    shippingAddress: '',
    paymentMethod: 'COD',
  } as CreateOrderRequest,
  validators: {
    onSubmit: createOrderSchema,
  },
  onSubmit: async ({ value }) => {
    const result = await createOrder({ ...value, couponCode: appliedCoupon.value?.code })

    if (result) {
      toast.success('Order placed successfully!')
      await cartStore.refreshCart()
      router.push('/orders')
    } else {
      toast.error(createOrderError.value ?? 'Failed to place order.')
    }
  },
})

function applyAddress(address: AddressDto) {
  selectedAddressId.value = address.id
  form.setFieldValue('receiverName', address.receiverName)
  form.setFieldValue('receiverPhone', address.receiverPhone)
  form.setFieldValue('shippingAddress', address.addressLine)
}

async function loadData() {
  await fetchCart()
  const ok = await fetchProfile()
  if (ok && profile.value) {
    const defaultAddress = profile.value.addresses.find((a) => a.isDefault) ?? profile.value.addresses[0]
    if (defaultAddress) {
      applyAddress(defaultAddress)
    }
  }
}

onMounted(loadData)
</script>
