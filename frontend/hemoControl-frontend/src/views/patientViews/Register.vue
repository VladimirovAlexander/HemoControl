<template>
  <div class="min-h-screen bg-blue-50 flex items-center justify-center p-8">
    <div class="bg-white shadow-lg rounded-2xl p-8 w-full max-w-sm">
      <h1 class="font-bold text-center mb-6 text-2xl">
        <span class="text-red-500">Гемо</span>
        <span class="text-blue-600">контроль</span>
      </h1>

      <p v-if="success" class="text-green-600 text-center mb-4">{{ success }}</p>

      <form @submit.prevent="register" class="space-y-6">
        <input 
          v-model="userName"
          type="text"
          placeholder="Имя пользователя"
          required
          class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        <input 
          v-model="email"
          type="email" 
          placeholder="Email" 
          required 
          class="w-full p-3 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        <input 
          v-model="policyNumber" 
          type="text" 
          placeholder="Полис" 
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
          {{ isLoading ? 'Регистрация...' : 'Зарегистрироваться' }}
        </button>
        <div class="text-center pt-4 border-t border-gray-200">
          <p class="text-gray-600">Уже есть аккаунт?</p>
          <router-link 
            to="/login" 
            class="text-blue-600 font-medium hover:text-blue-800 transition"
          >
            Войти
          </router-link>
        </div>
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
    const userName = ref('');
    const email = ref('');
    const password = ref('');
    const policyNumber = ref('');
    const error = ref('');
    const success = ref('');
    const isLoading = ref(false);
    const router = useRouter();

    const register = async () => {
      isLoading.value = true;
      error.value = '';
      success.value = '';
      try {
        const response = await fetch('https://localhost:7000/api/account/register', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            userName: userName.value,
            email: email.value,
            password: password.value,
            policyNumber: policyNumber.value, // строго как на сервере
          }),
        });

        const data = await response.text();

        if (response.status === 200 || response.status === 201) {
          success.value = 'Регистрация прошла успешно!';
          setTimeout(() => router.push('/hemocontrol'), 1500);
        } else {
          error.value = data || 'Ошибка при регистрации.';
        }
      } catch (err) {
        error.value = 'Ошибка при соединении с сервером.';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      userName,
      email,
      password,
      policyNumber,
      error,
      success,
      isLoading,
      register,
    };
  },
};
</script>
