<template>
  <div class="min-h-screen bg-blue-50 flex items-center justify-center p-8">
    <div class="bg-white shadow-lg rounded-2xl p-8 w-full max-w-sm">
      <h1 class="font-bold text-center mb-6 text-2xl">
        <span class="text-red-500">Гемо</span>
        <span class="text-blue-600">контроль</span>
      </h1>

      <p v-if="infoMessage" class="text-blue-600 text-center mb-4">
        {{ infoMessage }}
      </p>

      <form @submit.prevent="login" class="space-y-6">
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

        <button 
          type="submit" 
          :disabled="isLoading"
          class="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
        >
          {{ isLoading ? 'Вход...' : 'Войти' }}
        </button>
        <div class="text-center pt-4 border-t border-gray-200">
          <p class="text-gray-600">Ещё нет аккаунта?</p>
          <router-link 
            to="/personal-register" 
            class="text-blue-600 font-medium hover:text-blue-800 transition"
          >
            Зарегистрироваться
          </router-link>
        </div>
        <p v-if="error" class="text-red-500 text-center mt-4">{{ error }}</p>
      </form>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

export default {
  setup() {
    const email = ref('');
    const password = ref('');
    const error = ref('');
    const infoMessage = ref('');
    const isLoading = ref(false);
    const route = useRoute();
    const router = useRouter();
    const role = ref('');

    onMounted(() => {
      if (route.query.message) {
        infoMessage.value = route.query.message;
      }
    });

    const login = async () => {
      error.value = '';
      isLoading.value = true;
      try {
        const response = await axios.post('https://localhost:7000/api/account/login', {
          email: email.value,
          password: password.value
        });

        const token = response.data.token;
        localStorage.setItem('token', token);
        await getInfo();
        console.log(role.value);
        switch (role.value) {
          case 'Doctor':
            router.push('/doctor/home');
            break;
          case 'Assistant':
            router.push('/assistent/home');
            break;
          default:
            router.push('/admin/home');
            break;
        }
        infoMessage.value = 'Успешный вход!';
        error.value = '';
      } catch (err) {
        if (err.response) {
          error.value = err.response.data || 'Ошибка входа. Проверьте данные.';
        } else {
          error.value = 'Ошибка подключения к серверу.';
        }
      } finally {
        isLoading.value = false;
      }
    };

    const getInfo = async () => {
      try {
        const res = await axios.get('https://localhost:7000/api/account/info', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        role.value = res.data.role;
        localStorage.setItem('userId', res.data.userId);
      } catch (err) {
        console.error('Ошибка при получении аккаунта:', err);
      }
    };

    return {
      email,
      password,
      error,
      infoMessage,
      isLoading,
      login,
    };
  },
};
</script>
