<template>
  <div class="register-container">
    <h1>Register</h1>
    <form @submit.prevent="register">
      <div class="input-group">
        <input v-model="userName" type="text" placeholder="Username" required />
        <input v-model="email" type="email" placeholder="Email" required />
        <input v-model="password" type="password" placeholder="Password" required />
      </div>
      <button type="submit" :disabled="isLoading">
        {{ isLoading ? 'Registering...' : 'Register' }}
      </button>
      <p v-if="error" class="error-message">{{ error }}</p>
      <p v-if="success" class="success-message">{{ success }}</p>
    </form>
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
          body: JSON.stringify({ userName: userName.value, email: email.value, password: password.value }),
        });

        const data = await response.text();

        if (response.status === 201) {
          success.value = data || 'Registration successful.';
          setTimeout(() => router.push('/hemocontrol'), 1500);
        } else {
          error.value = data || 'Registration failed.';
        }
      } catch (err) {
        error.value = 'An error occurred during registration.';
      } finally {
        isLoading.value = false;
      }
    };

    return {
      userName,
      email,
      password,
      error,
      success,
      isLoading,
      register,
    };
  },
};
</script>

