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
        <h2 class="text-3xl font-semibold text-center text-blue-600 mb-6">Календарь приемов</h2>
  
        <!-- Календарь и окно событий -->
        <div class="flex">
          <!-- Календарь -->
          <div class="w-1/2 pr-6">
            <div class="flex justify-between items-center mb-4">
              <button @click="prevMonth" class="bg-gray-200 px-4 py-2 rounded hover:bg-gray-300">&lt;</button>
              <h2 class="text-xl font-semibold">{{ currentMonthName }} {{ currentYear }}</h2>
              <button @click="nextMonth" class="bg-gray-200 px-4 py-2 rounded hover:bg-gray-300">&gt;</button>
            </div>
  
            <div class="grid grid-cols-7 text-center font-semibold text-gray-600">
              <div v-for="day in weekDays" :key="day">{{ day }}</div>
            </div>
  
            <div class="grid grid-cols-7 gap-1 mt-2">
              <div v-for="day in calendarDays" :key="day.date" 
                   class="border h-10 flex items-center justify-center cursor-pointer relative hover:bg-blue-100 transition"
                   :class="{
                     'text-red-500': isToday(day.date),
                     'bg-blue-200': isSelected(day.date),
                     'text-gray-400': day.inactive
                   }"
                   @click="selectDay(day.date)">
                <span>{{ day.day }}</span>
                <div v-if="hasAppointments(day.date)" class="absolute bottom-1 right-1 bg-blue-500 w-2 h-2 rounded-full"></div>
              </div>
            </div>
          </div>
  
          <!-- Окно событий -->
          <div class="w-1/2 pl-6 border-l">
            <h3 class="text-xl font-semibold text-blue-600 mb-4">Записи на {{ selectedDate }}</h3>
            <ul class="divide-y">
              <li v-for="appointment in selectedAppointments" :key="appointment.id" class="p-4">
                <h4 class="text-lg font-semibold">{{ appointment.patientName }}</h4>
                <p class="text-gray-600">Время: {{ appointment.time }}</p>
              </li>
            </ul>
            <p v-if="!selectedAppointments.length" class="text-gray-500 text-center">Нет записей на этот день</p>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  import feather from 'feather-icons';
  
  export default {
    data() {
      const today = new Date().toISOString().split("T")[0];
      return {
        isLoggedIn: true,
        currentYear: new Date().getFullYear(),
        currentMonth: new Date().getMonth(),
        selectedDate: today, // По умолчанию выбрана сегодняшняя дата
        menuItems: [
          { title: "Мои пациенты", icon: "user", href: "/doctor/patients" },
          { title: "Запросы на анализы", icon: "file-text", href: "/doctor/lab-requests" },
          { title: "Записи на прием", icon: "calendar", href: "/doctor/appointments" },
          { title: "История болезней", icon: "clipboard", href: "/doctor/records" },
          { title: "Результаты анализов", icon: "bar-chart", href: "/doctor/lab-results" },
          { title: "Чат с лаборантами", icon: "message-circle", href: "/doctor/chat" },
        ],
        appointments: [
          { id: 1, patientName: "Иван Петров", date: "2025-03-21", time: "10:00" },
          { id: 2, patientName: "Ольга Иванова", date: "2025-03-22", time: "12:30" },
          { id: 3, patientName: "Сергей Смирнов", date: "2025-03-25", time: "15:00" },
          { id: 4, patientName: "Анна Козлова", date: "2025-03-26", time: "14:00" },
        ],
        weekDays: ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"],
      };
    },
    computed: {
      currentMonthName() {
        return new Date(this.currentYear, this.currentMonth).toLocaleString("ru", { month: "long" });
      },
      calendarDays() {
        let firstDayOfMonth = new Date(this.currentYear, this.currentMonth, 1);
        let lastDayOfMonth = new Date(this.currentYear, this.currentMonth + 1, 0);
        let days = [];
  
        let startDay = firstDayOfMonth.getDay() - 1;
        if (startDay === -1) startDay = 6; // Исправление для воскресенья
  
        for (let i = -startDay; i < lastDayOfMonth.getDate(); i++) {
          let date = new Date(this.currentYear, this.currentMonth, i + 1);
          days.push({
            date: date.toISOString().split("T")[0],
            day: i + 1 > 0 ? i + 1 : "",
            inactive: i + 1 <= 0,
          });
        }
        return days;
      },
      selectedAppointments() {
        return this.selectedDate ? this.appointments.filter(a => a.date === this.selectedDate) : [];
      }
    },
    methods: {
      goToLogin() {
        this.$router.push('/login');
      },
      goToProfile() {
        this.$router.push('/profile');
      },
      prevMonth() {
        if (this.currentMonth === 0) {
          this.currentMonth = 11;
          this.currentYear--;
        } else {
          this.currentMonth--;
        }
      },
      nextMonth() {
        if (this.currentMonth === 11) {
          this.currentMonth = 0;
          this.currentYear++;
        } else {
          this.currentMonth++;
        }
      },
      selectDay(date) {
        this.selectedDate = date;
      },
      hasAppointments(date) {
        return this.appointments.some(a => a.date === date);
      },
      isToday(date) {
        const today = new Date().toISOString().split("T")[0];
        return date === today;
      },
      isSelected(date) {
        return date === this.selectedDate;
      }
    },
    mounted() {
      if (typeof feather !== 'undefined') {
        feather.replace();
      } else {
        console.error("Feather icons не загружены!");
      }
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