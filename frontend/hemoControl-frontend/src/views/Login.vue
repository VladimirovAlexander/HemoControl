<template>
  <div class="min-h-screen bg-blue-50 flex items-center justify-center p-8">
    <div class="bg-white shadow-lg rounded-2xl p-8 w-1/4 h-90">
      
      <h1 class="font-bold text-center space-y-4 mb-6">
        <span class="text-red-500 text-2xl">Гемо</span>
        <span class="text-blue-600 text-2xl">контроль</span>
      </h1>

      <form @submit.prevent="login" class="space-y-6">
        
        <div class="space-y-4">
          <input 
            v-model="email"
            type="email" 
            placeholder="Email" 
            required 
            class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
          />
          <input 
            v-model="password" 
            type="password" 
            placeholder="Пароль" 
            required 
            class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
          />
        </div>

        <a href="https://your-link.com" class="text-blue-600 hover:underline">Восстановить</a>
        
        <button 
          type="submit" 
          :disabled="isLoading"
          class="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition disabled:opacity-50">
          {{ isLoading ? 'Вход...' : 'Войти' }}
        </button>

        <p class="text-center text-sm text-gray-600 mt-4">Войти другим способом</p>

        <button
          type="button"
          @click="loginWithTelegram"
          class="w-full bg-white text-blue-500 py-3 rounded-lg hover:bg-gray-200 transition mt-4 border border-blue-500">
          Войти через Telegram
        </button>
        
        <a href="https://your-link.com" class="text-blue-600 hover:underline block text-center mt-2">
          Не удаётся войти?
        </a>
        
        <p v-if="error" class="text-red-500 text-center mt-4">{{ error }}</p>

      </form>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useRouter } from 'vue-router';

export default {
  setup() {
    const email = ref('');
    const password = ref('');
    const error = ref('');
    const isLoading = ref(false);
    const router = useRouter();

    const login = async () => {
      error.value = ''; 
      isLoading.value = true;
      try {
        const response = await fetch('https://localhost:7000/api/account/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email: email.value, password: password.value }),
        });
        const data = await response.json();
        
        if (response.ok) {
          localStorage.setItem('token', data.token);
          router.push('/hemocontrol');
        } else {
          error.value = data.message || 'Ошибка входа. Проверьте данные.';
        }
      } catch (err) {
        error.value = 'Произошла ошибка подключения к серверу.';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      email,
      password,
      error,
      isLoading,
      login,
    };
  },
};
</script>
