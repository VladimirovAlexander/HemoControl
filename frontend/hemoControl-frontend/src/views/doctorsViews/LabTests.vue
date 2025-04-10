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
        <h2 class="text-3xl font-semibold text-center text-blue-600 mb-6">Направление на анализы</h2>
  
        <!-- Поиск пациента -->
        <div class="mb-6">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Введите имя пациента"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
  
        <!-- Список пациентов -->
        <div class="mb-6">
          <h3 class="text-xl font-semibold text-blue-600 mb-4">Выберите пациента</h3>
          <ul class="divide-y divide-gray-200">
            <li
              v-for="patient in filteredPatients"
              :key="patient.id"
              class="p-4 hover:bg-gray-50 cursor-pointer"
              :class="{ 'bg-blue-100': selectedPatient?.id === patient.id }"
              @click="selectPatient(patient)"
            >
              <div class="flex justify-between items-center">
                <div>
                  <p class="text-lg font-semibold">{{ patient.firstName }} {{ patient.lastName }}</p>
                  <p class="text-gray-600">Дата рождения: {{ patient.dateOfBirth }}</p>
                </div>
                <button
                  v-if="selectedPatient?.id === patient.id"
                  @click.stop="deselectPatient"
                  class="text-red-600 hover:text-red-800"
                >
                  Отменить выбор
                </button>
              </div>
            </li>
          </ul>
          <p v-if="filteredPatients.length === 0" class="text-gray-500 text-center mt-4">Пациенты не найдены</p>
        </div>
  
        <!-- Выбор анализов -->
        <div class="mb-6">
          <h3 class="text-xl font-semibold text-blue-600 mb-4">Выберите анализы</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="test in availableTests"
              :key="test.id"
              class="p-4 border border-gray-300 rounded-lg cursor-pointer hover:bg-blue-50"
              :class="{ 'bg-blue-100 border-blue-500': selectedTests.includes(test.id) }"
              @click="toggleTest(test.id)"
            >
              <p class="text-lg font-semibold">{{ test.name }}</p>
              <p class="text-gray-600">{{ test.description }}</p>
            </div>
          </div>
        </div>
  
        <!-- Кнопка отправки -->
        <div class="flex justify-end">
          <button
            @click="submitRequest"
            :disabled="!selectedPatient || selectedTests.length === 0"
            class="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition disabled:bg-gray-400 disabled:cursor-not-allowed"
          >
            Отправить направление
          </button>
        </div>
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
        selectedPatient: null, // Выбранный пациент
        selectedTests: [], // Выбранные анализы
        patients: [], // Список пациентов
        availableTests: [ // Список доступных анализов
          { id: 1, name: 'Общий анализ крови', description: 'Оценка общего состояния здоровья' },
          { id: 2, name: 'Биохимический анализ крови', description: 'Оценка функций органов' },
          { id: 3, name: 'Анализ мочи', description: 'Диагностика заболеваний почек' },
          { id: 4, name: 'Гормональный профиль', description: 'Оценка гормонального фона' },
          { id: 5, name: 'Коагулограмма', description: 'Оценка свертываемости крови' },
          { id: 6, name: 'Иммунограмма', description: 'Оценка иммунной системы' },
        ],
        menuItems: [
          { title: "Мои пациенты", icon: "user", href: "/doctor/patients" },
          { title: "Запросы на анализы", icon: "file-text", href: "/doctor/lab-requests" },
          { title: "Записи на прием", icon: "calendar", href: "/doctor/appointments" },
          { title: "История болезней", icon: "clipboard", href: "/doctor/records" },
          { title: "Результаты анализов", icon: "bar-chart", href: "/doctor/lab-results" },
          { title: "Чат с лаборантами", icon: "message-circle", href: "/doctor/chat" },
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
      // Выбор пациента
      selectPatient(patient) {
        this.selectedPatient = patient;
      },
      // Отмена выбора пациента
      deselectPatient() {
        this.selectedPatient = null;
      },
      // Выбор/отмена анализа
      toggleTest(testId) {
        if (this.selectedTests.includes(testId)) {
          this.selectedTests = this.selectedTests.filter(id => id !== testId);
        } else {
          this.selectedTests.push(testId);
        }
      },
      // Отправка направления
      async submitRequest() {
        if (!this.selectedPatient || this.selectedTests.length === 0) return;
  
        const requestData = {
          patientId: this.selectedPatient.id,
          tests: this.selectedTests,
        };
  
        try {
          const response = await axios.post('https://your-backend-api.com/lab-requests', requestData); // Замените на ваш URL
          alert('Направление успешно отправлено!');
          this.selectedPatient = null;
          this.selectedTests = [];
        } catch (error) {
          console.error('Ошибка при отправке направления:', error);
          alert('Произошла ошибка при отправке направления.');
        }
      },
      // Загрузка пациентов
      async fetchPatients() {
        try {
          const response = await axios.get('https://your-backend-api.com/patients'); // Замените на ваш URL
          this.patients = response.data;
        } catch (error) {
          console.error('Ошибка при загрузке пациентов:', error);
        }
      }
    },
    mounted() {
      if (typeof feather !== 'undefined') {
        feather.replace();
      } else {
        console.error("Feather icons не загружены!");
      }
      this.fetchPatients(); // Загружаем пациентов при монтировании компонента
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