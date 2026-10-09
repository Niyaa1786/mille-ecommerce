<script setup lang="ts">
import { ref, watch } from 'vue'
import { useForm } from '@tanstack/vue-form'
import { Plus, Upload, X, Trash2 } from 'lucide-vue-next'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Card, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useUpdateProduct } from '../composables/useUpdateProduct'
import { useUpdateProductImages } from '../composables/useUpdateProductImages'
import { useGetProduct } from '../composables/useGetProduct'
import {
  updateProductSchema,
  updateProductImagesSchema,
  PRODUCT_STATUS_LABELS,
  type UpdateProductRequest,
  type ProductStatus,
  type VariantRequest,
} from '../types/product'
import { useGetCategories } from '@/modules/categories/composables/useGetCategories'

const props = defineProps<{
  productId: string | null
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'success'): void
}>()

const { isLoading, errorMessage, errors, updateProduct } = useUpdateProduct()
const {
  isLoading: isImagesLoading,
  errorMessage: imagesErrorMessage,
  errors: imagesErrors,
  updateProductImages,
} = useUpdateProductImages()
const { product, fetchProduct } = useGetProduct()
const { categories, fetchCategories } = useGetCategories()

const STATUS_OPTIONS: ProductStatus[] = ['Active', 'OutOfStock', 'Contact', 'Discontinued']

const newImages = ref<File[]>([])
const imagesValidationError = ref<string | null>(null)

function emptyVariant(): VariantRequest {
  return { sku: '', price: 0, stock: 0, size: '', color: '' }
}

const form = useForm({
  defaultValues: {
    name: '',
    description: '',
    categoryId: 0,
    variants: [emptyVariant()],
    status: 'Active' as ProductStatus,
  } as UpdateProductRequest,
  validators: { onSubmit: updateProductSchema },
  onSubmit: async ({ value }) => {
    if (!props.productId) return
    const success = await updateProduct(props.productId, value)
    if (success) {
      emit('update:open', false)
      emit('success')
    }
  },
})

async function prefill(id: string) {
  await fetchProduct(id)
  if (!product.value) return
  form.reset()
  form.setFieldValue('name', product.value.name)
  form.setFieldValue('description', product.value.description ?? '')
  form.setFieldValue('categoryId', product.value.categoryId)
  form.setFieldValue('status', product.value.status)
  form.setFieldValue(
    'variants',
    product.value.variants.map((v) => ({
      sku: v.sku,
      price: v.price,
      stock: v.stock,
      size: v.size ?? '',
      color: v.color ?? '',
    })),
  )
}

watch(
  () => props.open,
  (isOpen) => {
    if (isOpen && props.productId) {
      newImages.value = []
      imagesValidationError.value = null
      prefill(props.productId)
      fetchCategories({ page: 1, pageSize: 100, includeDeleted: false })
    }
  },
)

function handleOpenChange(value: boolean) {
  emit('update:open', value)
}

function addVariant() {
  form.setFieldValue('variants', [...form.state.values.variants, emptyVariant()])
}

function removeVariant(idx: number) {
  const current = form.state.values.variants
  if (current.length <= 1) return
  form.setFieldValue(
    'variants',
    current.filter((_, i) => i !== idx),
  )
}

function handleFiles(event: Event) {
  const input = event.target as HTMLInputElement
  if (!input.files) return
  newImages.value = Array.from(input.files)
  imagesValidationError.value = null
  input.value = ''
}

function removeImage(idx: number) {
  newImages.value = newImages.value.filter((_, i) => i !== idx)
}

async function handleUpdateImages() {
  if (!props.productId) return

  const parsed = updateProductImagesSchema.safeParse({ images: newImages.value })
  if (!parsed.success) {
    imagesValidationError.value = parsed.error.issues[0]?.message ?? 'Invalid images'
    return
  }
  imagesValidationError.value = null

  const success = await updateProductImages(props.productId, newImages.value)
  if (success) {
    newImages.value = []
    await fetchProduct(props.productId)
    emit('success')
  }
}

function getErrorMessage(errs: any[]): string | undefined {
  if (!errs?.length) return undefined
  const e = errs[0]
  return typeof e === 'object' ? e?.message : e
}
</script>

<template>
  <Dialog :open="open" @update:open="handleOpenChange">
    <DialogContent class="sm:max-w-3xl max-h-[90vh] overflow-y-auto">
      <DialogHeader>
        <DialogTitle>Edit Product</DialogTitle>
        <DialogDescription> Update product details and images. They are saved separately. </DialogDescription>
      </DialogHeader>

      <form class="space-y-4" @submit.prevent.stop="form.handleSubmit">
        <!-- Name -->
        <form.Field name="name" v-slot="{ field, state }">
          <div class="grid gap-2">
            <Label :for="field.name">Name</Label>
            <Input
              :id="field.name"
              :model-value="field.state.value"
              placeholder="Product name"
              @update:model-value="(v) => field.handleChange(String(v))"
              @blur="field.handleBlur"
            />
            <p v-if="getErrorMessage(state.meta.errors)" class="text-xs text-destructive">
              {{ getErrorMessage(state.meta.errors) }}
            </p>
          </div>
        </form.Field>

        <!-- Description -->
        <form.Field name="description" v-slot="{ field }">
          <div class="grid gap-2">
            <Label :for="field.name">Description</Label>
            <Input
              :id="field.name"
              :model-value="field.state.value ?? ''"
              placeholder="Optional description"
              @update:model-value="(v) => field.handleChange(String(v))"
              @blur="field.handleBlur"
            />
          </div>
        </form.Field>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <!-- Category -->
          <form.Field name="categoryId" v-slot="{ field, state }">
            <div class="grid gap-2">
              <Label>Category</Label>
              <Select
                :model-value="field.state.value ? String(field.state.value) : ''"
                @update:model-value="(v) => field.handleChange(Number(v))"
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select category" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem v-for="cat in categories" :key="cat.id" :value="String(cat.id)">
                    {{ cat.name }}
                  </SelectItem>
                </SelectContent>
              </Select>
              <p v-if="getErrorMessage(state.meta.errors)" class="text-xs text-destructive">
                {{ getErrorMessage(state.meta.errors) }}
              </p>
            </div>
          </form.Field>

          <!-- Status -->
          <form.Field name="status" v-slot="{ field }">
            <div class="grid gap-2">
              <Label>Status</Label>
              <Select
                :model-value="field.state.value"
                @update:model-value="(v) => field.handleChange(v as ProductStatus)"
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem v-for="s in STATUS_OPTIONS" :key="s" :value="s">
                    {{ PRODUCT_STATUS_LABELS[s] }}
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>
          </form.Field>
        </div>

        <!-- Variants -->
        <div class="space-y-3">
          <div class="flex items-center justify-between">
            <Label>Variants</Label>
            <Button type="button" variant="outline" size="sm" @click="addVariant">
              <Plus class="mr-1 size-4" />
              Add Variant
            </Button>
          </div>

          <form.Field name="variants" v-slot="{ field, state }">
            <div class="space-y-3">
              <Card v-for="(_, idx) in field.state.value as VariantRequest[]" :key="idx" class="relative pt-4">
                <CardContent class="p-4 grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <Button
                    v-if="(field.state.value as VariantRequest[]).length > 1"
                    type="button"
                    variant="ghost"
                    size="icon"
                    class="absolute top-2 right-2 text-destructive hover:bg-destructive/10"
                    @click="removeVariant(idx)"
                  >
                    <Trash2 class="size-4" />
                  </Button>

                  <!-- SKU -->
                  <form.Field :name="`variants[${idx}].sku`" v-slot="{ field: f, state: s }">
                    <div class="grid gap-1.5">
                      <Label :for="f.name">SKU</Label>
                      <Input
                        :id="f.name"
                        :model-value="f.state.value"
                        placeholder="SKU-001"
                        @update:model-value="(v) => f.handleChange(String(v))"
                      />
                      <p v-if="getErrorMessage(s.meta.errors)" class="text-xs text-destructive">
                        {{ getErrorMessage(s.meta.errors) }}
                      </p>
                    </div>
                  </form.Field>

                  <!-- Price -->
                  <form.Field :name="`variants[${idx}].price`" v-slot="{ field: f, state: s }">
                    <div class="grid gap-1.5">
                      <Label :for="f.name">Price</Label>
                      <Input
                        :id="f.name"
                        type="number"
                        :model-value="f.state.value"
                        placeholder="0"
                        @update:model-value="(v) => f.handleChange(v === '' ? 0 : Number(v))"
                      />
                      <p v-if="getErrorMessage(s.meta.errors)" class="text-xs text-destructive">
                        {{ getErrorMessage(s.meta.errors) }}
                      </p>
                    </div>
                  </form.Field>

                  <!-- Stock -->
                  <form.Field :name="`variants[${idx}].stock`" v-slot="{ field: f, state: s }">
                    <div class="grid gap-1.5">
                      <Label :for="f.name">Stock</Label>
                      <Input
                        :id="f.name"
                        type="number"
                        :model-value="f.state.value"
                        placeholder="0"
                        @update:model-value="(v) => f.handleChange(v === '' ? 0 : Number(v))"
                      />
                      <p v-if="getErrorMessage(s.meta.errors)" class="text-xs text-destructive">
                        {{ getErrorMessage(s.meta.errors) }}
                      </p>
                    </div>
                  </form.Field>

                  <!-- Size -->
                  <form.Field :name="`variants[${idx}].size`" v-slot="{ field: f }">
                    <div class="grid gap-1.5">
                      <Label :for="f.name">Size (optional)</Label>
                      <Input
                        :id="f.name"
                        :model-value="f.state.value ?? ''"
                        placeholder="M / L / XL"
                        @update:model-value="(v) => f.handleChange(String(v))"
                      />
                    </div>
                  </form.Field>

                  <!-- Color -->
                  <form.Field :name="`variants[${idx}].color`" v-slot="{ field: f }">
                    <div class="grid gap-1.5 sm:col-span-2">
                      <Label :for="f.name">Color (optional)</Label>
                      <Input
                        :id="f.name"
                        :model-value="f.state.value ?? ''"
                        placeholder="Red / Black"
                        @update:model-value="(v) => f.handleChange(String(v))"
                      />
                    </div>
                  </form.Field>
                </CardContent>
              </Card>

              <p v-if="getErrorMessage(state.meta.errors)" class="text-xs text-destructive">
                {{ getErrorMessage(state.meta.errors) }}
              </p>
            </div>
          </form.Field>
        </div>

        <!-- Errors -->
        <div v-if="errorMessage || errors.length" class="rounded-md bg-destructive/15 p-3 text-xs text-destructive">
          <p v-if="errorMessage" class="font-medium">{{ errorMessage }}</p>
          <ul v-if="errors.length" class="list-disc pl-4 space-y-1 mt-1">
            <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
          </ul>
        </div>

        <div class="flex justify-end">
          <form.Subscribe v-slot="{ canSubmit }">
            <Button type="submit" :disabled="!canSubmit || isLoading">
              {{ isLoading ? 'Saving...' : 'Save Changes' }}
            </Button>
          </form.Subscribe>
        </div>
      </form>

      <!-- Images (separate API: PUT /api/Products/{id}/images) -->
      <div class="space-y-4 border-t pt-4">
        <div>
          <Label class="text-base">Images</Label>
          <p class="text-xs text-muted-foreground">Uploading new images replaces all current images.</p>
        </div>

        <!-- Current Images -->
        <div v-if="product?.images?.length" class="grid gap-2">
          <Label>Current Images</Label>
          <div class="flex flex-wrap gap-2">
            <div
              v-for="img in product.images"
              :key="img.id"
              class="relative size-20 rounded-md border overflow-hidden bg-muted/30"
            >
              <img :src="img.imageUrl" class="size-full object-cover" />
              <span
                v-if="img.isThumbnail"
                class="absolute bottom-0 inset-x-0 bg-black/60 text-white text-[10px] text-center py-0.5"
              >
                Thumbnail
              </span>
            </div>
          </div>
        </div>

        <!-- New images -->
        <div class="grid gap-2">
          <Label>Replace Images</Label>
          <div class="flex items-center gap-3">
            <Button type="button" variant="outline" as-child>
              <label class="cursor-pointer">
                <Upload class="mr-2 size-4" /> Choose new images
                <input type="file" accept="image/*" multiple class="hidden" @change="handleFiles" />
              </label>
            </Button>
            <span class="text-xs text-muted-foreground"> {{ newImages.length }} new file(s) selected </span>
          </div>

          <div v-if="newImages.length" class="flex flex-wrap gap-2 pt-1">
            <div
              v-for="(file, idx) in newImages"
              :key="idx"
              class="flex items-center gap-1.5 rounded-md border bg-muted/50 px-2.5 py-1 text-xs"
            >
              <span class="max-w-36 truncate">{{ file.name }}</span>
              <Button
                type="button"
                variant="ghost"
                size="icon"
                class="size-4 text-muted-foreground hover:text-destructive"
                @click="removeImage(idx)"
              >
                <X class="size-3" />
              </Button>
            </div>
          </div>

          <p v-if="imagesValidationError" class="text-xs text-destructive">
            {{ imagesValidationError }}
          </p>
        </div>

        <!-- Image errors -->
        <div
          v-if="imagesErrorMessage || imagesErrors.length"
          class="rounded-md bg-destructive/15 p-3 text-xs text-destructive"
        >
          <p v-if="imagesErrorMessage" class="font-medium">{{ imagesErrorMessage }}</p>
          <ul v-if="imagesErrors.length" class="list-disc pl-4 space-y-1 mt-1">
            <li v-for="(err, idx) in imagesErrors" :key="idx">{{ err }}</li>
          </ul>
        </div>

        <div class="flex justify-end">
          <Button type="button" :disabled="isImagesLoading || !newImages.length" @click="handleUpdateImages">
            {{ isImagesLoading ? 'Uploading...' : 'Update Images' }}
          </Button>
        </div>
      </div>

      <DialogFooter>
        <Button
          type="button"
          variant="outline"
          :disabled="isLoading || isImagesLoading"
          @click="handleOpenChange(false)"
        >
          Close
        </Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
