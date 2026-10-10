<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { toast } from 'vue-sonner'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'

import CreateReviewDialog from '@/modules/reviews/components/CreateReviewDialog.vue'
import { useGetOrder } from '../composables/useGetOrder'
import {
  ORDER_STATUS_LABELS,
  PAYMENT_METHOD_LABELS,
  PAYMENT_STATUS_LABELS,
  type OrderItemDto,
  type OrderStatus,
  type PaymentMethod,
  type PaymentStatus,
} from '../types/order'
import { formatCurrency, formatDate } from '@/shared/utils/format'

const props = defineProps<{
  orderId: string | null
  open: boolean
  allowReview?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
}>()

const { order, isLoading, errorMessage, fetchOrder } = useGetOrder()

watch(
  [() => props.open, () => props.orderId],
  ([isOpen, id]) => {
    if (isOpen && id) fetchOrder(id)
  },
  { immediate: true },
)

const showReview = computed(() => props.allowReview && order.value?.status === 'Completed')

// ===== Review dialog =====
const reviewOpen = ref(false)
const reviewItem = ref<OrderItemDto | null>(null)

function openReview(item: OrderItemDto) {
  reviewItem.value = item
  reviewOpen.value = true
}

function handleReviewSuccess() {
  toast.success('Review submitted successfully')
}

function handleOpenChange(value: boolean) {
  emit('update:open', value)
}

function statusLabel(status: string): string {
  return ORDER_STATUS_LABELS[status as OrderStatus] ?? status
}

function paymentLabel(method: string): string {
  return PAYMENT_METHOD_LABELS[method as PaymentMethod] ?? method
}

function paymentStatusLabel(status: string): string {
  return PAYMENT_STATUS_LABELS[status as PaymentStatus] ?? status
}
</script>

<template>
  <Dialog :open="open" @update:open="handleOpenChange">
    <DialogContent class="sm:max-w-3xl max-h-[90vh] overflow-y-auto">
      <DialogHeader>
        <DialogTitle>Order Details</DialogTitle>
        <DialogDescription>Full order information.</DialogDescription>
      </DialogHeader>

      <p v-if="errorMessage" class="text-sm text-destructive">{{ errorMessage }}</p>

      <div v-if="isLoading" class="space-y-4">
        <Skeleton class="h-6 w-1/2" />
        <Skeleton class="h-32 w-full" />
        <Skeleton class="h-24 w-full" />
      </div>

      <div v-else-if="order" class="space-y-5">
        <!-- Header: ID + Status -->
        <div class="flex items-center justify-between">
          <div>
            <p class="text-xs uppercase text-muted-foreground">Order ID</p>
            <p class="font-mono text-sm">{{ order.id }}</p>
          </div>
          <Badge variant="secondary">{{ statusLabel(order.status) }}</Badge>
        </div>

        <!-- Receiver info -->
        <div class="grid grid-cols-2 gap-4 text-sm">
          <div>
            <span class="text-muted-foreground text-xs uppercase">Receiver</span>
            <p class="font-medium">{{ order.receiverName }}</p>
          </div>
          <div>
            <span class="text-muted-foreground text-xs uppercase">Phone</span>
            <p class="font-medium">{{ order.receiverPhone }}</p>
          </div>
          <div class="col-span-2">
            <span class="text-muted-foreground text-xs uppercase">Shipping Address</span>
            <p class="font-medium">{{ order.shippingAddress }}</p>
          </div>
          <div>
            <span class="text-muted-foreground text-xs uppercase">Created At</span>
            <p class="font-medium">{{ formatDate(order.createdAt) }}</p>
          </div>
          <div>
            <span class="text-muted-foreground text-xs uppercase">Updated At</span>
            <p class="font-medium">{{ formatDate(order.updatedAt) }}</p>
          </div>
        </div>

        <Separator />

        <!-- Items -->
        <div class="space-y-2">
          <h4 class="font-medium">Items ({{ order.items?.length ?? 0 }})</h4>
          <div class="rounded-md border overflow-hidden">
            <Table>
              <TableHeader class="bg-muted/50">
                <TableRow>
                  <TableHead>SKU</TableHead>
                  <TableHead>Product</TableHead>
                  <TableHead class="text-right">Qty</TableHead>
                  <TableHead class="text-right">Unit Price</TableHead>
                  <TableHead class="text-right">Subtotal</TableHead>
                  <TableHead v-if="showReview" class="text-right">Review</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow v-for="item in order.items" :key="item.id">
                  <TableCell class="font-mono text-xs">{{ item.sku }}</TableCell>
                  <TableCell>{{ item.productName }}</TableCell>
                  <TableCell class="text-right">{{ item.quantity }}</TableCell>
                  <TableCell class="text-right">{{ formatCurrency(item.unitPrice) }}</TableCell>
                  <TableCell class="text-right font-medium">
                    {{ formatCurrency(item.subtotal) }}
                  </TableCell>
                  <TableCell v-if="showReview" class="text-right">
                    <Button variant="outline" size="sm" @click="openReview(item)">Review</Button>
                  </TableCell>
                </TableRow>

                <TableRow v-if="!order.items?.length">
                  <TableCell :colspan="showReview ? 6 : 5" class="h-20 text-center text-muted-foreground">
                    No items
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </div>

          <div class="flex justify-end gap-6 pt-2 text-sm">
            <div class="text-muted-foreground">
              Discount<template v-if="order.couponCode">
                (<span class="font-mono text-xs">{{ order.couponCode }}</span
                >)</template
              >:
              <span class="text-foreground">{{ formatCurrency(order.discountAmount) }}</span>
            </div>
            <div class="font-semibold">Total: {{ formatCurrency(order.totalAmount) }}</div>
          </div>
        </div>

        <!-- Payment -->
        <div v-if="order.payment" class="space-y-2">
          <h4 class="font-medium">Payment</h4>
          <div class="grid grid-cols-2 gap-4 text-sm">
            <div>
              <span class="text-muted-foreground text-xs uppercase">Method</span>
              <p class="font-medium">{{ paymentLabel(order.payment.method) }}</p>
            </div>
            <div>
              <span class="text-muted-foreground text-xs uppercase">Status</span>
              <p class="font-medium">{{ paymentStatusLabel(order.payment.status) }}</p>
            </div>
            <div>
              <span class="text-muted-foreground text-xs uppercase">Amount</span>
              <p class="font-medium">{{ formatCurrency(order.payment.amount) }}</p>
            </div>
            <div>
              <span class="text-muted-foreground text-xs uppercase">Paid At</span>
              <p class="font-medium">
                {{ order.payment.paidAt ? formatDate(order.payment.paidAt) : '—' }}
              </p>
            </div>
            <div v-if="order.payment.transactionId" class="col-span-2">
              <span class="text-muted-foreground text-xs uppercase">Transaction ID</span>
              <p class="font-mono text-xs">{{ order.payment.transactionId }}</p>
            </div>
          </div>
        </div>

        <!-- Status Histories -->
        <div class="space-y-2">
          <h4 class="font-medium">Status History</h4>
          <ol class="relative border-l border-border pl-4 space-y-3">
            <li v-for="history in order.statusHistories" :key="history.id" class="relative text-sm">
              <span class="absolute -left-5.25 top-1.5 size-2.5 rounded-full bg-primary" />
              <p class="font-medium">{{ statusLabel(history.status) }}</p>
              <p v-if="history.note" class="text-muted-foreground">{{ history.note }}</p>
              <p class="text-xs text-muted-foreground">{{ formatDate(history.createdAt) }}</p>
            </li>

            <li v-if="!order.statusHistories?.length" class="text-sm text-muted-foreground">No history</li>
          </ol>
        </div>
      </div>
    </DialogContent>
  </Dialog>

  <CreateReviewDialog v-model:open="reviewOpen" :order-item="reviewItem" @success="handleReviewSuccess" />
</template>
