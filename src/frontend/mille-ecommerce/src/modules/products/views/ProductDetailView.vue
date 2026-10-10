<template>
  <div>
    <!-- Loading -->
    <div v-if="isLoading" class="grid grid-cols-1 gap-8 md:grid-cols-2">
      <Skeleton class="aspect-square w-full rounded-lg" />
      <div class="flex flex-col gap-4">
        <Skeleton class="h-6 w-2/3" />
        <Skeleton class="h-4 w-1/3" />
        <Skeleton class="h-8 w-1/4" />
        <Skeleton class="h-24 w-full" />
      </div>
    </div>

    <!-- Error -->
    <div
      v-else-if="errorMessage"
      class="flex flex-col items-center justify-center gap-2 rounded-lg border border-dashed py-20 text-center"
    >
      <PackageX class="h-10 w-10 text-muted-foreground" />
      <p class="font-medium">{{ errorMessage }}</p>
      <Button variant="outline" size="sm" @click="loadProduct">Retry</Button>
    </div>

    <!-- Data -->
    <div v-else-if="product" class="flex flex-col gap-10">
      <div class="grid grid-cols-1 gap-8 md:grid-cols-2">
        <!-- Gallery -->
        <div class="flex flex-col gap-3">
          <div class="aspect-square overflow-hidden rounded-lg border bg-muted">
            <img v-if="activeImage" :src="activeImage" :alt="product.name" class="h-full w-full object-cover" />
            <div v-else class="flex h-full w-full items-center justify-center text-muted-foreground">
              <ImageOff class="h-10 w-10" />
            </div>
          </div>

          <div v-if="product.images.length > 1" class="flex gap-2 overflow-x-auto">
            <button
              v-for="image in product.images"
              :key="image.id"
              type="button"
              class="h-16 w-16 shrink-0 overflow-hidden rounded-md border"
              :class="activeImage === image.imageUrl ? 'ring-2 ring-primary' : ''"
              @click="activeImage = image.imageUrl"
            >
              <img :src="image.imageUrl" :alt="product.name" class="h-full w-full object-cover" />
            </button>
          </div>
        </div>

        <!-- Info -->
        <div class="flex flex-col gap-4">
          <div>
            <span class="text-sm text-muted-foreground">{{ product.categoryName }}</span>
            <h1 class="text-2xl font-bold">{{ product.name }}</h1>
          </div>

          <Badge v-if="product.status !== 'Active'" variant="secondary" class="w-fit">
            {{ PRODUCT_STATUS_LABELS[product.status] }}
          </Badge>

          <div class="text-2xl font-semibold">
            {{
              product.status === 'Contact' ? 'Please contact us' : formatCurrency(selectedVariant?.price ?? minPrice)
            }}
          </div>

          <!-- Variant selection -->
          <div v-if="hasSizes" class="space-y-2">
            <h3 class="text-sm font-medium">Size</h3>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="size in sizes"
                :key="size"
                type="button"
                class="rounded-md border px-3 py-1.5 text-sm transition-colors"
                :class="
                  selectedSize === size
                    ? 'border-foreground bg-foreground text-background'
                    : 'hover:border-foreground/50'
                "
                @click="selectedSize = size"
              >
                {{ size }}
              </button>
            </div>
          </div>

          <div v-if="hasColors" class="space-y-2">
            <h3 class="text-sm font-medium">Color</h3>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="color in colors"
                :key="color"
                type="button"
                class="rounded-md border px-3 py-1.5 text-sm transition-colors"
                :class="
                  selectedColor === color
                    ? 'border-foreground bg-foreground text-background'
                    : 'hover:border-foreground/50'
                "
                @click="selectedColor = color"
              >
                {{ color }}
              </button>
            </div>
          </div>

          <p v-if="!selectedVariant" class="text-sm text-destructive">This combination is currently unavailable.</p>
          <p v-else-if="selectedVariant.stock === 0" class="text-sm text-destructive">Out of stock.</p>
          <p v-else class="text-sm text-muted-foreground">{{ selectedVariant.stock }} item(s) in stock</p>

          <!-- Quantity -->
          <div class="flex items-center gap-3">
            <span class="text-sm font-medium">Quantity</span>
            <div class="flex items-center rounded-md border">
              <Button type="button" variant="ghost" size="icon-sm" :disabled="quantity <= 1" @click="quantity--">
                <Minus class="h-4 w-4" />
              </Button>
              <span class="w-10 text-center text-sm">{{ quantity }}</span>
              <Button
                type="button"
                variant="ghost"
                size="icon-sm"
                :disabled="!!selectedVariant && quantity >= selectedVariant.stock"
                @click="quantity++"
              >
                <Plus class="h-4 w-4" />
              </Button>
            </div>
          </div>

          <p v-if="addToCartError" class="text-sm text-destructive">{{ addToCartError }}</p>

          <Button class="w-fit" size="lg" :disabled="!canAddToCart || isAddingToCart" @click="handleAddToCart">
            <ShoppingCart class="h-4 w-4" />
            {{ isAddingToCart ? 'Adding...' : 'Add to cart' }}
          </Button>
        </div>
      </div>

      <!-- Description -->
      <div v-if="product.description" class="border-t pt-6">
        <h2 class="mb-3 text-lg font-semibold">Product description</h2>
        <p class="whitespace-pre-line text-sm text-muted-foreground">{{ product.description }}</p>
      </div>

      <!-- Reviews -->
      <ProductReviews :product-id="product.id" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { toast } from 'vue-sonner'
import { ImageOff, Minus, PackageX, Plus, ShoppingCart } from 'lucide-vue-next'

import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'

import { useGetProduct } from '../composables/useGetProduct'
import { useAddToCart } from '@/modules/carts/composables/useAddToCart'
import ProductReviews from '@/modules/reviews/components/ProductReviews.vue'
import { PRODUCT_STATUS_LABELS } from '../types/product'
import { formatCurrency } from '@/shared/utils/format'

const route = useRoute()
const { product, isLoading, errorMessage, fetchProduct } = useGetProduct()
const { isLoading: isAddingToCart, errorMessage: addToCartError, addToCart } = useAddToCart()

const activeImage = ref<string | null>(null)
const selectedSize = ref<string | null>(null)
const selectedColor = ref<string | null>(null)
const quantity = ref(1)

const sizes = computed(() => {
  if (!product.value) return []
  return [...new Set(product.value.variants.map((v) => v.size).filter(Boolean))] as string[]
})

const colors = computed(() => {
  if (!product.value) return []
  return [...new Set(product.value.variants.map((v) => v.color).filter(Boolean))] as string[]
})

const hasSizes = computed(() => sizes.value.length > 0)
const hasColors = computed(() => colors.value.length > 0)

const minPrice = computed(() => {
  if (!product.value || product.value.variants.length === 0) return 0
  return Math.min(...product.value.variants.map((v) => v.price))
})

const selectedVariant = computed(() => {
  if (!product.value) return null
  return (
    product.value.variants.find((v) => {
      const sizeMatches = hasSizes.value ? v.size === selectedSize.value : true
      const colorMatches = hasColors.value ? v.color === selectedColor.value : true
      return sizeMatches && colorMatches
    }) ?? null
  )
})

const canAddToCart = computed(() => !!selectedVariant.value && selectedVariant.value.stock > 0 && quantity.value > 0)

function initSelection() {
  if (!product.value) return

  activeImage.value =
    product.value.images.find((img) => img.isThumbnail)?.imageUrl ?? product.value.images[0]?.imageUrl ?? null

  selectedSize.value = product.value.variants[0]?.size ?? null
  selectedColor.value = product.value.variants[0]?.color ?? null
  quantity.value = 1
}

async function loadProduct() {
  const id = route.params.id as string
  await fetchProduct(id)
  initSelection()
}

watch(selectedVariant, () => {
  quantity.value = 1
})

async function handleAddToCart() {
  if (!selectedVariant.value) return

  const success = await addToCart({
    productVariantId: selectedVariant.value.id,
    quantity: quantity.value,
  })

  if (success) {
    toast.success(`Added "${product.value?.name}" to your cart`)
  } else {
    toast.error(addToCartError.value ?? 'Failed to add item to cart.')
  }
}

onMounted(loadProduct)
</script>
