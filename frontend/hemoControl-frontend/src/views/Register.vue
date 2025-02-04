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

<style>
.register-container {
  max-width: 400px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  border: 1px solid #ddd;
  border-radius: 8px;
  text-align: center;
}

.input-group {
  margin-bottom: 15px;
}

input {
  width: 100%;
  padding: 10px;
  margin: 5px 0;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
}

button {
  background-color: #4caf50;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background-color: #aaa;
  cursor: not-allowed;
}

.error-message {
  color: red;
  margin-top: 10px;
}

.success-message {
  color: green;
  margin-top: 10px;
}
</style>