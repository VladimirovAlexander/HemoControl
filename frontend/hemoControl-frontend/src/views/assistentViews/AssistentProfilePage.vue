<template>
  <div class="max-w-5xl mx-auto p-6 space-y-6">
    <!-- Карточка с аватаром -->
    <div class="bg-white shadow rounded-xl p-6 text-center">
      <img
        class="w-32 h-32 rounded-full mx-auto border border-gray-300"
        src="/default-avatar.png"
        alt="Avatar"
      />
      <h2 class="mt-4 text-xl font-bold text-gray-800">
        {{ profile.accountName || 'Доктор' }}
      </h2>
      <p class="text-gray-500 mt-1">{{ profile.specialization || 'Специализация не указана' }}</p>
    </div>

    <!-- Карточка: Учётная запись -->
    <div class="bg-white shadow rounded-xl p-6">
      <h3 class="text-lg font-semibold mb-4 text-gray-700">Учётная запись</h3>
      <div class="space-y-3 text-gray-600">
        <p><strong>Email:</strong> {{ profile.email }}</p>
        <p><strong>Телефон:</strong> {{ profile.phone }}</p>
      </div>
    </div>

    <!-- Карточка: Личная информация -->
    <div class="bg-white shadow rounded-xl p-6">
      <h3 class="text-lg font-semibold mb-4 text-gray-700">Личная информация</h3>

      <div class="space-y-4">
        <input v-model="profile.name" placeholder="Имя" class="form-input" />
        <input v-model="profile.surname" placeholder="Фамилия" class="form-input" />
        <input v-model="profile.patronymic" placeholder="Отчество" class="form-input" />
        <input v-model="profile.specialization" placeholder="Специализация" class="form-input" />
        <input v-model="profile.experience" placeholder="Стаж (лет)" class="form-input" />
        <textarea v-model="profile.bio" placeholder="О себе" class="form-input h-24 resize-none"></textarea>

        <button @click="updateProfile" class="bg-green-600 text-white px-4 py-2 rounded-lg">
          Сохранить изменения
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'DoctorProfilePage',
  data() {
    return {
      profile: {
        accountName: '',
        email: '',
        phone: '',
        name: '',
        surname: '',
        patronymic: '',
        specialization: '',
        experience: '',
        bio: '',
      }
    };
  },
  methods: {
    async getDoctorProfile() {
      try {
        const res = await axios.get('https://localhost:7000/api/account/info', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });

        if (res.data) {
          this.profile.accountName = res.data.accountName;
          this.profile.email = res.data.email;
          this.profile.phone = res.data.phone;
          // Предположим, что это API тоже возвращает doctor data:
          this.profile.name = res.data.name || '';
          this.profile.surname = res.data.surname || '';
          this.profile.patronymic = res.data.patronymic || '';
          this.profile.specialization = res.data.specialization || '';
          this.profile.experience = res.data.experience || '';
          this.profile.bio = res.data.bio || '';
        }
      } catch (err) {
        console.error('Ошибка загрузки данных профиля врача:', err);
      }
    },
    async updateProfile() {
      try {
        await axios.put(
          'https://localhost:7000/api/doctor/update-profile',
          {
            name: this.profile.name,
            surname: this.profile.surname,
            patronymic: this.profile.patronymic,
            specialization: this.profile.specialization,
            experience: this.profile.experience,
            bio: this.profile.bio
          },
          {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          }
        );
        alert('Профиль врача обновлён');
      } catch (err) {
        console.error('Ошибка обновления профиля врача:', err);
        alert('Ошибка обновления профиля');
      }
    }
  },
  mounted() {
    this.getDoctorProfile();
  }
};
</script>

<style scoped>
.form-input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  outline: none;
  transition: border 0.2s;
}
.form-input:focus {
  border-color: #3b82f6;
}
</style>
