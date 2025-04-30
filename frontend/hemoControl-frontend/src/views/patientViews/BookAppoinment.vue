<template>
  <div class="p-6 max-w-7xl mx-auto">
    <div class="flex gap-8">

      <!-- Выбор врача -->
      <div class="w-1/3 bg-white shadow-xl rounded-2xl p-6">
        <h2 class="text-2xl font-bold text-blue-800 mb-4 text-center">Выбор врача</h2>
        <div class="mb-4">
          <label class="block text-gray-700 font-medium mb-1">Выберите врача</label>
          <select
            v-model="selectedDoctorId"
            @change="onDoctorChange"
            class="w-full border rounded-lg px-4 py-2"
          >
            <option disabled value="">-- Выберите врача --</option>
            <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
              {{ doctor.lastName }} {{ doctor.firstName }}
            </option>
          </select>
        </div>
      </div>

      <!-- Календарь и время -->
      <div class="w-2/3 bg-white shadow-xl rounded-2xl p-6">
        <h2 class="text-2xl font-bold text-blue-800 mb-4 text-center">Выбор даты и времени</h2>

        <!-- Календарь -->
        <div class="mb-6">
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
              class="border h-10 flex items-center justify-center cursor-pointer relative transition"
              :class="{
                'text-red-500': isToday(day.date),
                'bg-blue-200': isSelected(day.date),
                'text-gray-400 cursor-not-allowed': !day.active,
                'hover:bg-blue-100': day.active
              }"
              @click="selectDay(day)"
            >
              <span>{{ day.day }}</span>
            </div>
          </div>
        </div>

        <!-- Время -->
        <div v-if="selectedDate && availableTimes.length" class="mb-6">
          <h2 class="text-xl font-bold text-blue-800 mb-4 text-center">Выберите время</h2>
          <div class="flex flex-wrap gap-2 justify-center">
            <button
              v-for="time in availableTimes"
              :key="time"
              @click="selectTime(time)"
              :class="[ 'px-4 py-2 text-sm rounded-lg border transition',
                selectedTime === time ? 'bg-blue-600 text-white border-blue-600' : 'bg-gray-100 hover:bg-blue-50' ]"
            >
              {{ time }}
            </button>
          </div>
        </div>

        <!-- Кнопка записи -->
        <div v-if="selectedDoctorId && selectedDate && selectedTime" class="mt-6 text-center">
          <button
            @click="bookAppointment"
            class="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg"
          >
            Записаться
          </button>
        </div>

        <!-- Сообщение об успешной записи -->
        <div v-if="successMessage" class="fixed inset-0 bg-gray-900 bg-opacity-50 flex justify-center items-center z-50">
          <div class="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full">
            <h3 class="text-xl font-bold text-center text-green-600 mb-4">Успешная запись</h3>
            <p class="text-center mb-4">{{ successMessage }}</p>
            <div class="text-center">
              <button @click="closeModal" class="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg">
                Закрыть
              </button>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script>
import { format, parseISO } from 'date-fns'
import { ru } from 'date-fns/locale'
import axios from 'axios'

export default {
  data() {
    return {
      doctors: [],
      selectedDoctorId: '',
      availableDates: [], // от сервера
      selectedDate: '',
      availableTimes: [],
      selectedTime: '',
      successMessage: '',
      currentMonth: new Date().getMonth(),
      currentYear: new Date().getFullYear(),
      weekDays: ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'],
    }
  },
  computed: {
    currentMonthName() {
      const months = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь']
      return months[this.currentMonth]
    },
    calendarDays() {
      const startOfMonth = new Date(this.currentYear, this.currentMonth, 1)
      const endOfMonth = new Date(this.currentYear, this.currentMonth + 1, 0)

      const days = []
      for (let i = 1; i <= endOfMonth.getDate(); i++) {
        const dateObj = new Date(this.currentYear, this.currentMonth, i)
        const dateStr = format(dateObj, 'yyyy-MM-dd')
        days.push({
          day: i,
          date: dateStr,
          active: this.availableDates.includes(dateStr),
        })
      }
      return days
    }
  },
  mounted() {
    this.fetchDoctors()
  },
  methods: {
    async fetchDoctors() {
      try {
        const response = await axios.get('http://localhost:5044/api/doctor/GetDoctors')
        this.doctors = response.data
      } catch (error) {
        console.error('Ошибка получения врачей:', error)
      }
    },
    async onDoctorChange() {
      this.selectedDate = ''
      this.selectedTime = ''
      this.availableTimes = []
      this.successMessage = ''

      if (!this.selectedDoctorId) return

      try {
        const response = await axios.get(`http://localhost:5044/api/schedule/available-days/${this.selectedDoctorId}`)
        this.availableDates = response.data
      } catch (error) {
        console.error('Ошибка получения доступных дней:', error)
      }
    },
    async selectDay(day) {
      if (!day.active) return

      this.selectedDate = day.date
      this.selectedTime = ''
      try {
        const response = await axios.get(`http://localhost:5044/api/schedule/available-times/${this.selectedDoctorId}`, {
          params: { date: this.selectedDate }
        })
        this.availableTimes = response.data
      } catch (error) {
        console.error('Ошибка получения доступного времени:', error)
      }
    },
    selectTime(time) {
      this.selectedTime = time
    },
    prevMonth() {
      this.currentMonth = (this.currentMonth - 1 + 12) % 12
      if (this.currentMonth === 11) this.currentYear -= 1
    },
    nextMonth() {
      this.currentMonth = (this.currentMonth + 1) % 12
      if (this.currentMonth === 0) this.currentYear += 1
    },
    isToday(date) {
      const today = format(new Date(), 'yyyy-MM-dd')
      return date === today
    },
    isSelected(date) {
      return this.selectedDate === date
    },
    async bookAppointment() {
      if (!this.selectedDoctorId || !this.selectedDate || !this.selectedTime) return;

      const appointmentDateTime = new Date(`${this.selectedDate}T${this.selectedTime}:00Z`);

      const dto = {
        doctorId: this.selectedDoctorId,
        appointmentDate: appointmentDateTime.toISOString() // важно для PostgreSQL
      };

      try {
        const response = await axios.post('http://localhost:5044/api/reception/createReception', dto);

        const doctor = this.doctors.find(d => d.id === this.selectedDoctorId);
        this.successMessage = `Вы записались к врачу ${doctor.lastName} ${doctor.firstName} на ${format(parseISO(this.selectedDate), "d MMMM", { locale: ru })} в ${this.selectedTime}`;
      } catch (error) {
        console.error('Ошибка при создании записи:', error);
        alert('Произошла ошибка при записи. Пожалуйста, попробуйте позже.');
      }
    },
    closeModal() {
      this.successMessage = ''
      this.selectedDoctorId = ''
      this.selectedDate = ''
      this.selectedTime = ''
      this.availableTimes = []
      this.availableDates = []
      this.$router.push('/appointments')
    }
  }
}
</script>


<style scoped>
/* Добавление стилизации */
.card {
  padding: 16px;
  border-radius: 8px;
  background-color: #fff;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.card-header {
  text-align: center;
  font-weight: bold;
  margin-bottom: 16px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
}

.grid-item {
  text-align: center;
  padding: 8px;
  cursor: pointer;
  border-radius: 4px;
}

.grid-item:hover {
  background-color: #f0f4f8;
}

.selected {
  background-color: #c3d8ff;
}
</style>
