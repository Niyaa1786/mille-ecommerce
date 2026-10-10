<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Trash2, SquarePen } from 'lucide-vue-next'

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Skeleton } from '@/components/ui/skeleton'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

import CreateCouponDialog from '@/modules/coupons/components/CreateCouponDialog.vue'
import EditCouponDialog from '@/modules/coupons/components/EditCouponDialog.vue'
import DeleteCouponDialog from '@/modules/coupons/components/DeleteCouponDialog.vue'

import { useGetCoupons } from '@/modules/coupons/composables/useGetCoupons'
import {
  DISCOUNT_TYPE_LABELS,
  type Coupon,
  type DiscountType,
} from '@/modules/coupons/types/coupon'
import { formatCurrency, formatDate } from '@/shared/utils/format'

const { coupons, pagination, isLoading, errorMessage, fetchCoupons } = useGetCoupons()

type ActiveFilter = 'All' | 'Active' | 'Inactive'

const page = ref(1)
const pageSize = ref(10)
const keyword = ref('')
const activeFilter = ref<ActiveFilter>('All')

function loadCoupons() {
  fetchCoupons({
    page: page.value,
    pageSize: pageSize.value,
    keyword: keyword.value || undefined,
    isActive: activeFilter.value === 'All' ? undefined : activeFilter.value === 'Active',
  })
}

function goToPage(next: number) {
  if (next < 1 || next > pagination.value.totalPages) return
  page.value = next
  loadCoupons()
}

function onSearch() {
  page.value = 1
  loadCoupons()
}

function onActiveChange(value: ActiveFilter) {
  activeFilter.value = value
  page.value = 1
  loadCoupons()
}

function discountLabel(coupon: Coupon): string {
  if (coupon.discountType === 'Percentage') return `${coupon.discountValue}%`
  return formatCurrency(coupon.discountValue)
}

function discountTypeLabel(type: string): string {
  return DISCOUNT_TYPE_LABELS[type as DiscountType] ?? type
}

const editOpen = ref(false)
const editingCoupon = ref<Coupon | null>(null)

function openEdit(coupon: Coupon) {
  editingCoupon.value = coupon
  editOpen.value = true
}

const deleteOpen = ref(false)
const deletingCoupon = ref<Coupon | null>(null)

function openDelete(coupon: Coupon) {
  deletingCoupon.value = coupon
  deleteOpen.value = true
}

onMounted(loadCoupons)
</script>

<template>
  <div class="p-6 space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold">Coupons</h1>
        <p class="text-sm text-muted-foreground">Manage your discount coupons.</p>
      </div>
      <CreateCouponDialog @success="loadCoupons" />
    </div>

    <!-- Filters -->
    <div class="flex flex-wrap items-center gap-3">
      <div class="flex items-center gap-2 max-w-sm flex-1 min-w-60">
        <Input v-model="keyword" placeholder="Search by code..." @keyup.enter="onSearch" />
        <Button variant="outline" @click="onSearch">Search</Button>
      </div>

      <Select
        :model-value="activeFilter"
        @update:model-value="(v) => onActiveChange(v as ActiveFilter)"
      >
        <SelectTrigger class="w-44">
          <SelectValue placeholder="All statuses" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="All">All statuses</SelectItem>
          <SelectItem value="Active">Active</SelectItem>
          <SelectItem value="Inactive">Inactive</SelectItem>
        </SelectContent>
      </Select>
    </div>

    <!-- Error -->
    <p v-if="errorMessage" class="text-sm text-destructive">{{ errorMessage }}</p>

    <!-- Table -->
    <div class="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Code</TableHead>
            <TableHead>Discount</TableHead>
            <TableHead class="text-right">Min Order</TableHead>
            <TableHead class="text-right">Max Discount</TableHead>
            <TableHead class="text-right">Used</TableHead>
            <TableHead>Start Date</TableHead>
            <TableHead>End Date</TableHead>
            <TableHead>Status</TableHead>
            <TableHead class="w-35 text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>

        <TableBody>
          <!-- Loading -->
          <template v-if="isLoading">
            <TableRow v-for="i in 5" :key="`sk-${i}`">
              <TableCell v-for="c in 9" :key="c"><Skeleton class="h-4 w-full" /></TableCell>
            </TableRow>
          </template>

          <!-- Data -->
          <template v-else-if="coupons.length">
            <TableRow v-for="coupon in coupons" :key="coupon.id">
              <TableCell>
                <div class="font-mono font-medium">{{ coupon.code }}</div>
                <div class="max-w-60 truncate text-xs text-muted-foreground">
                  {{ coupon.description || '—' }}
                </div>
              </TableCell>
              <TableCell>
                <div class="font-medium">{{ discountLabel(coupon) }}</div>
                <div class="text-xs text-muted-foreground">
                  {{ discountTypeLabel(coupon.discountType) }}
                </div>
              </TableCell>
              <TableCell class="text-right">
                {{ coupon.minOrderAmount !== null ? formatCurrency(coupon.minOrderAmount) : '—' }}
              </TableCell>
              <TableCell class="text-right">
                {{
                  coupon.maxDiscountAmount !== null ? formatCurrency(coupon.maxDiscountAmount) : '—'
                }}
              </TableCell>
              <TableCell class="text-right">
                {{ coupon.usedCount }} / {{ coupon.quantity ?? '∞' }}
              </TableCell>
              <TableCell>{{ coupon.startDate ? formatDate(coupon.startDate) : '—' }}</TableCell>
              <TableCell>{{ coupon.endDate ? formatDate(coupon.endDate) : '—' }}</TableCell>
              <TableCell>
                <Badge :variant="coupon.isActive ? 'default' : 'outline'">
                  {{ coupon.isActive ? 'Active' : 'Inactive' }}
                </Badge>
              </TableCell>
              <TableCell class="text-right">
                <div class="flex justify-end gap-2">
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    title="Edit coupon"
                    @click="openEdit(coupon)"
                  >
                    <SquarePen class="size-4 text-muted-foreground" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    title="Delete coupon"
                    @click="openDelete(coupon)"
                  >
                    <Trash2 class="size-4 text-destructive" />
                  </Button>
                </div>
              </TableCell>
            </TableRow>
          </template>

          <!-- Empty -->
          <template v-else>
            <TableRow>
              <TableCell colspan="9" class="h-24 text-center text-muted-foreground">
                No coupons found.
              </TableCell>
            </TableRow>
          </template>
        </TableBody>
      </Table>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between text-sm">
      <span class="text-muted-foreground">
        Page {{ pagination.page }} of {{ pagination.totalPages }} —
        {{ pagination.totalCount }} items
      </span>

      <div class="flex gap-2">
        <Button
          variant="outline"
          size="sm"
          :disabled="pagination.page <= 1 || isLoading"
          @click="goToPage(pagination.page - 1)"
        >
          Previous
        </Button>
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

    <!-- Edit / Delete Dialogs -->
    <EditCouponDialog v-model:open="editOpen" :coupon="editingCoupon" @success="loadCoupons" />
    <DeleteCouponDialog v-model:open="deleteOpen" :coupon="deletingCoupon" @success="loadCoupons" />
  </div>
</template>
