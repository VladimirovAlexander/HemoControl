<template>
    <div class="p-6 max-w-3xl mx-auto">
      <h2 class="text-2xl font-bold mb-6 text-blue-800">Запись к врачу</h2>
  
      <!-- Шаг 1: Выбор специализации -->
      <div class="mb-6">
        <label class="block text-sm font-medium text-gray-700 mb-1">Специализация</label>
        <select v-model="selectedSpecialty" @change="filterDoctors"
                class="w-full p-3 border border-gray-300 rounded">
          <option disabled value="">Выберите специализацию</option>
          <option v-for="spec in specialties" :key="spec">{{ spec }}</option>
        </select>
      </div>
  
      <!-- Шаг 2: Выбор врача -->
      <div class="mb-6" v-if="filteredDoctors.length">
        <label class="block text-sm font-medium text-gray-700 mb-1">Врач</label>
        <select v-model="selectedDoctor" @change="loadSchedule"
                class="w-full p-3 border border-gray-300 rounded">
          <option disabled value="">Выберите врача</option>
          <option v-for="doc in filteredDoctors" :key="doc.name">{{ doc.name }}</option>
        </select>
      </div>
  
      <!-- Шаг 3: Выбор времени -->
      <div class="mb-6" v-if="availableSlots.length">
        <label class="block text-sm font-medium text-gray-700 mb-2">Выберите время приёма</label>
        <div class="grid grid-cols-2 gap-4">
          <button
            v-for="slot in availableSlots"
            :key="slot"
            :class="[
              'p-3 rounded border',
              selectedTime === slot ? 'bg-blue-600 text-white' : 'bg-white hover:bg-blue-50'
            ]"
            @click="selectedTime = slot"
          >
            {{ slot }}
          </button>
        </div>
      </div>
  
      <!-- Кнопка -->
      <div v-if="selectedDoctor && selectedTime">
        <button
          @click="bookAppointment"
          class="mt-4 bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded transition"
        >
          Записаться
        </button>
      </div>
  
      <!-- Успешное сообщение -->
      <div v-if="successMessage" class="mt-6 text-green-600 font-medium">
        {{ successMessage }}
      </div>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        specialties: ['Гематолог', 'Терапевт', 'Кардиолог'],
        doctors: [
          { name: 'Иванов И.И.', specialty: 'Гематолог' },
          { name: 'Петрова А.А.', specialty: 'Гематолог' },
          { name: 'Сидоров С.С.', specialty: 'Терапевт' },
          { name: 'Кузнецова Н.Н.', specialty: 'Кардиолог' },
        ],
        selectedSpecialty: '',
        filteredDoctors: [],
        selectedDoctor: '',
        availableSlots: [],
        selectedTime: '',
        successMessage: ''
      };
    },
    methods: {
      filterDoctors() {
        this.filteredDoctors = this.doctors.filter(
          doc => doc.specialty === this.selectedSpecialty
        );
        this.selectedDoctor = '';
        this.availableSlots = [];
        this.selectedTime = '';
        this.successMessage = '';
      },
      loadSchedule() {
        // Пример данных — можно заменить на API-запрос
        this.availableSlots = [
          '15 апреля, 10:00',
          '15 апреля, 12:00',
          '16 апреля, 09:30',
          '16 апреля, 14:00',
        ];
        this.selectedTime = '';
        this.successMessage = '';
      },
      bookAppointment() {
        // Здесь должен быть запрос к серверу
        this.successMessage = `Вы успешно записались к врачу ${this.selectedDoctor} на ${this.selectedTime}`;
      }
    }
  };
  </script>
  
  <style scoped>
  select {
    appearance: none;
    background-position: right 0.5rem center;
    background-repeat: no-repeat;
  }
  </style>
  