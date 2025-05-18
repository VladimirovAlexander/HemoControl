<template>
  <div class="max-w-5xl mx-auto p-6 space-y-6">
    <!-- Карточка с аватаром и синхронизацией -->
    <div class="bg-white shadow rounded-xl p-6 text-center">
      <img
        class="w-32 h-32 rounded-full mx-auto border border-gray-300"
        src="/default-avatar.png"
        alt="Avatar"
      />
      <h2 class="mt-4 text-xl font-bold text-gray-800">
        {{ profile.accountName || 'Имя пользователя' }}
      </h2>
    </div>

    <!-- Карточка: Учётная запись -->
    <div class="bg-white shadow rounded-xl p-6">
      <h3 class="text-lg font-semibold mb-4 text-gray-700">Учётная запись</h3>
      <div class="space-y-3 text-gray-600">
        <p><strong>Email:</strong> {{ profile.email }}</p>
        <p><strong>Телефон:</strong> {{ profile.phone }}</p>
      </div>
    </div>

    <!-- Карточка: Информация о пациенте -->
    <div class="bg-white shadow rounded-xl p-6">
      <h3 class="text-lg font-semibold mb-4 text-gray-700">Информация о пациенте</h3>

      <div v-if="!isSynced">
        <button
          class="mt-4 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm"
          @click="syncWithPatient"
        >
          Синхронизировать
        </button>
      </div>

      <div v-else>
        <p class="text-green-600 font-semibold mb-4">
          Синхронизировано с пациентом: {{ profile.accountName }}
        </p>

        <div class="space-y-3">
          <input v-model="profile.name" placeholder="Имя" class="form-input" />
          <input v-model="profile.surname" placeholder="Фамилия" class="form-input" />
          <input v-model="profile.patronymic" placeholder="Отчество" class="form-input" />
          <input v-model="profile.dateOfBirth" type="date" class="form-input" />
          <input v-model="profile.country" placeholder="Страна" class="form-input" />
          <input v-model="profile.region" placeholder="Регион" class="form-input" />
          <input v-model="profile.city" placeholder="Город" class="form-input" />
          <input v-model="profile.street" placeholder="Улица" class="form-input" />
          <input v-model.number="profile.houseNumber" placeholder="Дом" class="form-input" />
          <input v-model.number="profile.appartmentNumber" placeholder="Квартира" class="form-input" />

          <button @click="updateProfile" class="bg-green-600 text-white px-4 py-2 rounded-lg">
            Сохранить
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'UserProfile',
  data() {
    return {
      profile: {
        accountName: '',
        policy: '',
        name: '',
        surname: '',
        patronymic: '',
        dateOfBirth: '',
        country: '',
        region: '',
        city: '',
        street: '',
        houseNumber: '',
        appartmentNumber: '',
        userId: '',
        patientId: '',
        email: '',
        phone: '',
      },
      isSynced: false,
    };
  },
  methods: {
    async getAccountInfo(){
      try{
        const res = await axios.get('https://localhost:7000/api/account/info', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        if (res.data){
          this.profile.accountName = res.data.accountName;
          this.profile.phone = res.data.phone;
          this.profile.email = res.data.email;
        }
      }
      catch (err) {
        console.error('Ошибка при получении информации о аккаунте:', err);
      }
    },
    async checkSyncStatus() {
      try {
        const res = await axios.get(`https://localhost:7098/api/patient/is-synced/${localStorage.getItem('userId')}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });

        if (res.data && res.data.patientId) {
          this.isSynced = true;
          this.profile.patientId = res.data.patientId;
          localStorage.setItem('patientId', res.data.patientId);
          if (this.profile.patientId) {
            await this.fetchExtendedInfo();
          }
        }
      } catch (err) {
        console.error('Ошибка при проверке синхронизации:', err);
      }
    },

    async syncWithPatient() {
      try {
        const res = await axios.get(`https://localhost:7098/api/patient/sync-user-with-patient`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });

        if (res.status === 200 && res.data.patientId) {
          this.profile.patientId = res.data.patientId;
          localStorage.setItem('patientId', res.data.patientId);
          this.isSynced = true;
          await this.fetchExtendedInfo();
        } else {
          alert('Ошибка синхронизации. Не получен ID пациента.');
        }
      } catch (err) {
        console.error('Ошибка синхронизации:', err);
      }
    },

    async fetchExtendedInfo() {
      if (!this.profile.patientId) return;

      try {
        const res = await axios.get(`https://localhost:7098/api/patient/${this.profile.patientId}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });

        this.profile = {
          ...this.profile,
          name: res.data.name,
          surname: res.data.surname,
          patronymic: res.data.patronymic,
          dateOfBirth: res.data.dateOfBirth,
          country: res.data.country,
          region: res.data.region,
          city: res.data.city,
          street: res.data.street,
          houseNumber: res.data.houseNumber,
          appartmentNumber: res.data.appartmentNumber,
        };
      } catch (err) {
        console.error('Ошибка при получении расширенных данных:', err);
      }
    },

    async updateProfile() {
      if (!this.profile.patientId) return;

      try {
        await axios.put(
          `https://localhost:7098/api/patient/update/${this.profile.patientId}`,
          {
            surname: this.profile.surname,
            patronymic: this.profile.patronymic,
            dateOfBirth: this.profile.dateOfBirth,
            country: this.profile.country,
            region: this.profile.region,
            city: this.profile.city,
            street: this.profile.street,
            houseNumber: this.profile.houseNumber,
            appartmentNumber: this.profile.appartmentNumber,
            name: this.profile.name,
          },
          {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
          }
        );
        alert('Профиль успешно обновлён.');
      } catch (err) {
        console.error('Ошибка при обновлении профиля:', err);
      }
    },
  },
  mounted() {
    const storedUserId = localStorage.getItem('userId');
    const storedPatientId = localStorage.getItem('patientId');

    if (storedUserId) {
      this.profile.userId = storedUserId;
    }

    if (storedPatientId) {
      this.profile.patientId = storedPatientId;
      this.isSynced = true;
    }

    this.checkSyncStatus();
    this.getAccountInfo();
  },
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
