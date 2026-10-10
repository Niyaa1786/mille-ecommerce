<template>
  <div class="mx-auto max-w-4xl">
    <div class="mb-6 flex flex-wrap items-center justify-between gap-3">
      <h1 class="text-2xl font-bold">My Orders</h1>

      <Select :model-value="statusFilter" @update:model-value="(v) => onStatusChange(v as OrderStatus | 'All')">
        <SelectTrigger class="w-44">
          <SelectValue placeholder="All statuses" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="All">All statuses</SelectItem>
          <SelectItem v-for="s in STATUS_OPTIONS" :key="s" :value="s">
            {{ ORDER_STATUS_LABELS[s] }}
          </SelectItem>
        </SelectContent>
      </Select>
    </div>

    <!-- Error -->
    <p v-if="errorMessage" class="mb-4 text-sm text-destructive">{{ errorMessage }}</p>

    <!-- Loading -->
    <div v-if="isLoading" class="flex flex-col gap-4">
      <Skeleton v-for="n in 3" :key="n" class="h-36 w-full rounded-lg" />
    </div>

    <!-- Empty -->
    <div
      v-else-if="orders.length === 0"
      class="flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed py-24 text-center"
    >
      <PackageSearch class="h-12 w-12 text-muted-foreground" />
      <p class="font-medium">You have no orders yet</p>
      <p class="text-sm text-muted-foreground">Once you place an order, it will show up here.</p>
      <RouterLink to="/products">
        <Button class="mt-2">Start shopping</Button>
      </RouterLink>
    </div>

    <!-- Data -->
    <div v-else class="flex flex-col gap-4">
      <Card v-for="order in orders" :key="order.id">
        <CardHeader class="flex flex-row flex-wrap items-start justify-between gap-2">
          <div>
            <CardTitle class="font-mono text-sm"> #{{ order.id.slice(0, 8).toUpperCase() }} </CardTitle>
            <CardDescription>{{ formatDate(order.createdAt) }}</CardDescription>
          </div>
          <Badge :variant="statusVariant(order.status)">{{ statusLabel(order.status) }}</Badge>
        </CardHeader>

        <CardContent class="space-y-2 text-sm">
          <div class="flex justify-between">
            <span class="text-muted-foreground">Receiver</span>
            <span class="font-medium">{{ order.receiverName }} · {{ order.receiverPhone }}</span>
          </div>
          <div class="flex justify-between gap-4">
            <span class="shrink-0 text-muted-foreground">Shipping address</span>
            <span class="text-right">{{ order.shippingAddress }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">Payment</span>
            <span>
              {{ paymentLabel(order.paymentMethod) }} ·
              {{ paymentStatusLabel(order.paymentStatus) }}
            </span>
          </div>
          <Separator class="my-2" />
          <div class="flex items-center justify-between font-semibold">
            <span>Total</span>
            <span>{{ formatCurrency(order.totalAmount) }}</span>
          </div>
        </CardContent>

        <CardFooter class="flex justify-end gap-2">
          <Button
            v-if="canCancel(order)"
            variant="outline"
            size="sm"
            class="hover:border-destructive hover:text-destructive"
            @click="openCancelDialog(order)"
          >
            Cancel order
          </Button>
          <Button variant="outline" size="sm" @click="openDetail(order)">View details</Button>
        </CardFooter>
      </Card>

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

    <DetailOrderDialog v-model:open="detailOpen" :order-id="detailOrderId" allow-review />

    <CancelOrderDialog v-model:open="cancelDialogOpen" :order="orderToCancel" @success="handleCancelSuccess" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { toast } from 'vue-sonner'
import { PackageSearch } from 'lucide-vue-next'

import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'
import { Separator } from '@/components/ui/separator'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'

import CancelOrderDialog from '../components/CancelOrderDialog.vue'
import DetailOrderDialog from '../components/DetailOrderDialog.vue'
import { useGetMyOrders } from '../composables/useGetMyOrders'
import {
  ORDER_STATUS_LABELS,
  PAYMENT_METHOD_LABELS,
  PAYMENT_STATUS_LABELS,
  type OrderStatus,
  type OrderSummaryDto,
  type PaymentMethod,
  type PaymentStatus,
} from '../types/order'
import { formatCurrency, formatDate } from '@/shared/utils/format'

const { orders, pagination, isLoading, errorMessage, fetchMyOrders } = useGetMyOrders()

const STATUS_OPTIONS: OrderStatus[] = ['Pending', 'Confirmed', 'Shipping', 'Completed', 'Cancelled']
const statusFilter = ref<OrderStatus | 'All'>('All')

async function loadOrders() {
  await fetchMyOrders({
    page: pagination.value.page,
    pageSize: pagination.value.pageSize || 10,
    status: statusFilter.value === 'All' ? undefined : statusFilter.value,
  })
}

function goToPage(next: number) {
  if (next < 1 || next > pagination.value.totalPages) return
  pagination.value.page = next
  loadOrders()
}

function onStatusChange(value: OrderStatus | 'All') {
  statusFilter.value = value
  pagination.value.page = 1
  loadOrders()
}

function statusLabel(status: string): string {
  return ORDER_STATUS_LABELS[status as OrderStatus] ?? status
}

function statusVariant(status: string): 'default' | 'secondary' | 'destructive' | 'outline' {
  switch (status) {
    case 'Completed':
      return 'default'
    case 'Cancelled':
      return 'destructive'
    case 'Shipping':
      return 'secondary'
    default:
      return 'outline'
  }
}

function paymentLabel(method: string): string {
  return PAYMENT_METHOD_LABELS[method as PaymentMethod] ?? method
}

function paymentStatusLabel(status: string): string {
  return PAYMENT_STATUS_LABELS[status as PaymentStatus] ?? status
}

function canCancel(order: OrderSummaryDto): boolean {
  return order.status === 'Pending'
}

// ===== Cancel confirm dialog =====
const cancelDialogOpen = ref(false)
const orderToCancel = ref<OrderSummaryDto | null>(null)

function openCancelDialog(order: OrderSummaryDto) {
  orderToCancel.value = order
  cancelDialogOpen.value = true
}

async function handleCancelSuccess() {
  toast.success('Order cancelled successfully')
  orderToCancel.value = null
  await loadOrders()
}

// ===== Detail dialog =====
const detailOpen = ref(false)
const detailOrderId = ref<string | null>(null)
function openDetail(order: OrderSummaryDto) {
  detailOrderId.value = order.id
  detailOpen.value = true
}

onMounted(loadOrders)
</script>
