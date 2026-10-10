import { useAuthStore } from '@/modules/auth/stores/authStore'
import LoginView from '@/modules/auth/views/LoginView.vue'
import RegisterView from '@/modules/auth/views/RegisterView.vue'
import { useCartStore } from '@/modules/carts/stores/cartStore'
import CartView from '@/modules/carts/views/CartView.vue'
import CategoryList from '@/modules/categories/views/CategoryList.vue'
import CouponList from '@/modules/coupons/views/CouponList.vue'
import CheckoutView from '@/modules/orders/views/CheckoutView.vue'
import MyOrdersView from '@/modules/orders/views/MyOrdersView.vue'
import OrderList from '@/modules/orders/views/OrderList.vue'
import ProductDetailView from '@/modules/products/views/ProductDetailView.vue'
import ProductList from '@/modules/products/views/ProductList.vue'
import ProductsView from '@/modules/products/views/ProductsView.vue'
import ProfileView from '@/modules/users/views/ProfileView.vue'
import Home from '@/shared/components/Home.vue'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    //Auth Routes
    {
      path: '/',
      name: 'HomeView',
      component: Home,
      meta: { layout: 'storefront' },
    },
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
      meta: {
        layout: 'auth',
      },
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterView,
      meta: {
        layout: 'auth',
      },
    },
    //Admin Routes
    {
      path: '/admin/categories',
      name: 'CategoryList',
      component: CategoryList,
      meta: {
        layout: 'admin',
        requiresAuth: true,
      },
    },
    {
      path: '/admin/products',
      name: 'ProductList',
      component: ProductList,
      meta: {
        layout: 'admin',
        requiresAuth: true,
      },
    },
    {
      path: '/admin/orders',
      name: 'OrderList',
      component: OrderList,
      meta: {
        layout: 'admin',
        requiresAuth: true,
      },
    },
    {
      path: '/admin/coupons',
      name: 'CouponList',
      component: CouponList,
      meta: {
        layout: 'admin',
        requiresAuth: true,
      },
    },
    //Storefront
    {
      path: '/profile',
      name: 'Profile',
      component: ProfileView,
      meta: { layout: 'storefront', requiresAuth: true },
    },
    {
      path: '/products',
      name: 'Products',
      component: ProductsView,
      meta: { layout: 'storefront' },
    },
    {
      path: '/products/:id',
      name: 'ProductDetail',
      component: ProductDetailView,
      meta: { layout: 'storefront' },
    },
    {
      path: '/cart',
      name: 'Cart',
      component: CartView,
      meta: { layout: 'storefront' },
    },
    {
      path: '/checkout',
      name: 'Checkout',
      component: CheckoutView,
      meta: { layout: 'storefront', requiresAuth: true },
    },
    {
      path: '/orders',
      name: 'MyOrders',
      component: MyOrdersView,
      meta: { layout: 'storefront', requiresAuth: true },
    },
  ],
})

router.beforeEach((to, from) => {
  const authStore = useAuthStore()
  const cartStore = useCartStore()
  authStore.restoreSession()
  cartStore.restoreCart()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'Login' }
  }
  return true
})

export default router
