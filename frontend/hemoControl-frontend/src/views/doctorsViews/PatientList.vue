<template>
    <div class="bg-blue-50 min-h-screen">
      <!-- Верхняя панель -->
      <div class="bg-white text-black flex items-center justify-between py-4 px-8 shadow-md">
        <h1 class="text-xl font-bold">
          <span class="text-red-500 text-2xl">Гемо</span>
          <span class="text-blue-600 text-2xl">контроль</span>
        </h1>
  
        <div v-if="isLoggedIn">
          <img src="" alt="Профиль" class="rounded-full w-8 h-8 cursor-pointer" @click="goToProfile">
        </div>
        <button v-else @click="goToLogin" class="text-1xl bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
          Войти
        </button>
      </div>
  
      <!-- Навигация -->
      <nav class="bg-blue-600 p-4 text-white flex justify-center shadow">
        <div class="flex space-x-10">
          <router-link
            v-for="item in menuItems"
            :key="item.title"
            :to="item.href"
            class="flex flex-col items-center justify-center text-center w-24 h-24 hover:underline"
          >
            <i :data-feather="item.icon" class="mb-2 text-2xl"></i>
            <span class="text-sm">{{ item.title }}</span>
          </router-link>
        </div>
      </nav>
  
      <!-- Основной контент -->
      <div class="bg-white shadow-md rounded-lg p-6 mt-8 max-w-7xl mx-auto">
        <h2 class="text-3xl font-semibold text-center text-blue-600 mb-6">Поиск пациентов</h2>
  
        <!-- Поисковая строка -->
        <div class="mb-6">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Введите имя пациента"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
  
        <!-- Таблица пациентов -->
        <div class="overflow-x-auto">
          <table class="min-w-full bg-white border border-gray-300">
            <thead>
              <tr>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Имя</th>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Фамилия</th>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Дата рождения</th>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Пол</th>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Действия</th>
                <th class="px-6 py-3 border-b-2 border-gray-300 text-left text-sm font-semibold text-gray-700">Последний прием</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="patient in filteredPatients" :key="patient.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 border-b border-gray-300">{{ patient.firstName }}</td>
                <td class="px-6 py-4 border-b border-gray-300">{{ patient.lastName }}</td>
                <td class="px-6 py-4 border-b border-gray-300">{{ patient.dateOfBirth }}</td>
                <td class="px-6 py-4 border-b border-gray-300">{{ patient.gender }}</td>
                <td class="px-6 py-4 border-b border-gray-300">
                  <button @click="viewPatient(patient.id)" class="text-blue-600 hover:text-blue-800">Просмотр</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
  
        <!-- Сообщение, если пациенты не найдены -->
        <p v-if="filteredPatients.length === 0" class="text-gray-500 text-center mt-6">Пациенты не найдены</p>
      </div>
    </div>
  </template>
  
  <script>
  import feather from 'feather-icons';
  import axios from 'axios';
  
  export default {
    data() {
      return {
        isLoggedIn: true,
        searchQuery: '',
        patients: [], // Изначально пустой массив пациентов
        menuItems: [
            { title: "Главная", icon: "home", href: "/doctor/dashboard"},
            { title: "Мои пациенты", icon: "user", href: "/doctor/patients" },
            { title: "Направления на анализы", icon: "file-text", href: "/doctor/lab-requests" },
            { title: "Записи на прием", icon: "calendar", href: "/doctor/appointments" },
        ],
      };
    },
    computed: {
      // Фильтрация пациентов по поисковому запросу
      filteredPatients() {
        return this.patients.filter(patient => {
          const fullName = `${patient.firstName} ${patient.lastName}`.toLowerCase();
          return fullName.includes(this.searchQuery.toLowerCase());
        });
      }
    },
    methods: {
      goToLogin() {
        this.$router.push('/login');
      },
      goToProfile() {
        this.$router.push('/profile');
      },
      viewPatient(patientId) {
        this.$router.push(`/doctor/patients/${patientId}`);
      },
      // Метод для загрузки пациентов
      async fetchPatients() {
        try {
          const response = await axios.get('https://your-backend-api.com/patients'); // Замените на ваш URL
          this.patients = response.data; // Обновляем данные
        } catch (error) {
          console.error('Ошибка при загрузке данных:', error);
        }
      }
    },
    mounted() {
      if (typeof feather !== 'undefined') {
        feather.replace();
      } else {
        console.error("Feather icons не загружены!");
      }
      this.fetchPatients(); // Загружаем данные при монтировании компонента
    },
  };
  </script>
  
  <style scoped>
  .grid {
    display: grid;
    gap: 4px;
  }
  
  button {
    transition: all 0.3s ease;
  }
  
  button:hover {
    background-color: #3b82f6;
    color: white;
  }
  </style>