<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Пример вычисляемого свойства (можно удалить, если не нужно)
const fullname = computed(() => "HemoControl User");

// Функция для выхода из системы
const logout = () => {
  localStorage.removeItem('token'); // Удаляем токен
  router.push('/login'); // Перенаправляем на страницу входа
};

// Проверка, авторизован ли пользователь
const isAuthenticated = computed(() => !!localStorage.getItem('token'));
</script>

<template>
  <header>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="125" height="125" />

    <nav>
      <router-link to="/login" v-if="!isAuthenticated">Login</router-link>
      <router-link to="/register" v-if="!isAuthenticated">Register</router-link>
      <router-link to="/hemocontrol" v-if="isAuthenticated"></router-link>
      <button @click="logout" v-if="isAuthenticated">Logout</button>
    </nav>
  </header>

  <main>
    <router-view></router-view> <!-- Здесь отображаются страницы -->
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #f0f0f0;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

nav {
  display: flex;
  gap: 1rem;
}

nav a {
  text-decoration: none;
  color: #2c3e50;
  font-weight: bold;
}

nav a.router-link-exact-active {
  color: #42b983;
}

button {
  padding: 0.5rem 1rem;
  background-color: #ff4757;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #ff6b81;
}

@media (min-width: 1024px) {
  header {
    padding: 2rem;
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  nav {
    gap: 2rem;
  }
}
</style>