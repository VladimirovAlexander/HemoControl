<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Пример вычисляемого свойства (можно удалить, если не нужно)
const fullname = computed(() => "HemoControl User");

// Проверка, авторизован ли пользователь
const isAuthenticated = computed(() => !!localStorage.getItem('token'));
</script>

<template>
  <main>
    <router-view></router-view> <!-- Здесь отображаются страницы -->
  </main>
</template>

