<template>
  <SidebarProvider>
    <Sidebar collapsible="icon">
      <SidebarHeader class="border-b border-sidebar-border">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" as-child>
              <RouterLink to="/admin/products">
                <div
                  class="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground"
                >
                  <Store class="size-4" />
                </div>
                <div class="grid flex-1 text-left text-sm leading-tight">
                  <span class="truncate font-semibold">Mille Store</span>
                  <span class="truncate text-xs text-muted-foreground">Admin Panel</span>
                </div>
              </RouterLink>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Management</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem v-for="item in navItems" :key="item.to">
                <SidebarMenuButton as-child :tooltip="item.label" :is-active="isActive(item.to)">
                  <RouterLink :to="item.to">
                    <component :is="item.icon" />
                    <span>{{ item.label }}</span>
                  </RouterLink>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter class="border-t border-sidebar-border">
        <SidebarMenu>
          <!-- Back to Store -->
          <SidebarMenuItem>
            <SidebarMenuButton as-child tooltip="Back to Store">
              <RouterLink to="/">
                <ArrowLeft />
                <span>Back to Store</span>
              </RouterLink>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <!-- Logout -->
          <SidebarMenuItem>
            <SidebarMenuButton
              tooltip="Logout"
              class="text-destructive hover:bg-destructive/10 hover:text-destructive"
              @click="handleLogout"
            >
              <LogOut />
              <span>Logout</span>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>

      <SidebarRail />
    </Sidebar>

    <SidebarInset>
      <header class="flex h-14 shrink-0 items-center gap-2 border-b border-border bg-background/95 px-4 backdrop-blur">
        <SidebarTrigger class="-ml-1" />
        <Separator orientation="vertical" class="mr-2 h-4" />
        <h1 class="text-sm font-medium">{{ currentPageTitle }}</h1>

        <Button as-child variant="ghost" size="sm" class="ml-auto">
          <RouterLink to="/">
            <ArrowLeft class="mr-1 size-4" />
            Back to Store
          </RouterLink>
        </Button>
      </header>

      <div class="flex flex-1 flex-col gap-4 p-4">
        <slot />
      </div>
    </SidebarInset>
  </SidebarProvider>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { Package, Tags, ShoppingBag, Ticket, LogOut, Store, ArrowLeft } from 'lucide-vue-next'

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarInset,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarRail,
  SidebarTrigger,
} from '@/components/ui/sidebar'
import { Separator } from '@/components/ui/separator'
import { Button } from '@/components/ui/button'
import { useLogout } from '@/modules/auth/composables/useLogout'

const route = useRoute()
const { handleLogout } = useLogout()

interface NavItem {
  label: string
  to: string
  icon: any
}

const navItems: NavItem[] = [
  { label: 'Products', to: '/admin/products', icon: Package },
  { label: 'Categories', to: '/admin/categories', icon: Tags },
  { label: 'Orders', to: '/admin/orders', icon: ShoppingBag },
  { label: 'Coupons', to: '/admin/coupons', icon: Ticket },
]

function isActive(to: string): boolean {
  return route.path === to || route.path.startsWith(`${to}/`)
}

const currentPageTitle = computed(() => {
  const match = navItems.find((i) => isActive(i.to))
  return match?.label ?? 'Admin'
})
</script>
