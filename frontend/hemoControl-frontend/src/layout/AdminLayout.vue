<!-- layouts/AppLayout.vue -->
<template>
    <div class="flex min-h-screen bg-blue-50 text-black">
      <!-- Sidebar -->
      <aside
        :class="[
          'bg-blue-700 text-white transition-all duration-300 ease-in-out',
          isSidebarCollapsed ? 'w-20' : 'w-64'
        ]"
        class="flex flex-col"
      >
        <!-- Логотип -->
        <div class="flex items-center justify-between p-4 border-b border-blue-600">
          <h1 class="text-xl font-bold whitespace-nowrap overflow-hidden" v-if="!isSidebarCollapsed">
            <span class="text-red-400">Гемо</span><span class="text-white">контроль</span>
          </h1>
          <button @click="toggleSidebar" class="text-white focus:outline-none">
            <i data-feather="chevron-left" :class="{ 'rotate-180': isSidebarCollapsed }"></i>
          </button>
        </div>
  
        <!-- Навигация -->
        <nav class="flex-1 py-4 flex flex-col space-y-2">
          <router-link
            v-for="item in menuItems"
            :key="item.title"
            :to="item.href"
            class="flex items-center space-x-3 px-4 py-3 hover:bg-blue-600 transition rounded"
          >
            <i :data-feather="item.icon" class="text-xl shrink-0"></i>
            <span v-if="!isSidebarCollapsed" class="text-sm">{{ item.title }}</span>
          </router-link>
        </nav>
      </aside>
  
      <!-- Контент -->
      <div class="flex-1 flex flex-col">
        <!-- Верхняя панель -->
        <header class="bg-white shadow flex justify-between items-center px-6 py-4">
          <h2 class="text-2xl font-semibold">Гемоконтроль</h2>
          <div v-if="isLoggedIn" class="flex items-center space-x-4">
            <img
              src="https://placehold.co/40x40"
              alt="Профиль"
              class="rounded-full w-8 h-8 cursor-pointer"
              @click="goToProfile"
            />
            <button @click="logout" class="text-red-500 text-sm hover:underline">Выйти</button>
          </div>
          <button
            v-else
            @click="goToLogin"
            class="text-sm bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
          >
            Войти
          </button>
        </header>
  
        <!-- Основной контент -->
        <main class="flex-1 p-6 overflow-y-auto">
          <router-view />
        </main>
      </div>
    </div>
  </template>
  
  <script>
  import { onMounted, nextTick } from 'vue';
  import feather from 'feather-icons';
  
  export default {
    data() {
      return {
        isSidebarCollapsed: false,
        isLoggedIn: false,
        menuItems: [
          { title: 'Главная', icon: 'home', href: '/admin/home' },
        ]
      };
    },
    methods: {
      toggleSidebar() {
        this.isSidebarCollapsed = !this.isSidebarCollapsed;
        nextTick(() => feather.replace());
      },
      goToLogin() {
        this.$router.push('/personal-login');
      },
      goToProfile() {
        this.$router.push('/admin/profile');
      },
      logout() {
        localStorage.removeItem('token');
        localStorage.clear();
        this.isLoggedIn = false;
        this.$router.push('/personal-login');
      },
      checkAuth() {
        const token = localStorage.getItem('token');
        this.isLoggedIn = !!token;
      },
    },
    mounted() {
      this.checkAuth();
      nextTick(() => feather.replace());
    },
  };
  </script>
  
  <style scoped>
  .rotate-180 {
    transform: rotate(180deg);
    transition: transform 0.3s ease;
  }
  </style>
  