<template>
  <div class="p-6 max-w-7xl mx-auto">
    <div class="flex gap-8">
      <!-- Список врачей -->
      <div class="w-1/3 bg-white shadow-xl rounded-2xl p-6">
        <h2 class="text-2xl font-bold text-blue-800 mb-4 text-center">Список врачей</h2>
        <div class="mb-4">
          <label class="block text-gray-700 font-medium mb-1">Выберите врача</label>
          <select
            v-model="selectedDoctorId"
            class="w-full border rounded-lg px-4 py-2"
          >
            <option disabled value="">-- Выберите врача --</option>
            <option v-for="doctor in doctors" :key="doctor.id" :value="doctor.id">
              {{ doctor.lastName }} {{ doctor.firstName }} ({{ doctor.specialization }})
            </option>
          </select>
        </div>
        
        <div v-if="selectedDoctorId" class="mt-6">
          <h3 class="text-lg font-semibold mb-2">Последние созданные слоты</h3>
          <div v-if="doctorSlots.length > 0" class="space-y-2">
            <div v-for="slot in doctorSlots" :key="slot.id" class="border p-2 rounded">
              <p>{{ formatDate(slot.startDateTime) }} - {{ formatTime(slot.startDateTime) }}</p>
            </div>
          </div>
          <p v-else class="text-gray-500">Нет созданных слотов</p>
        </div>
      </div>

      <!-- Создание слотов -->
      <div class="w-2/3 bg-white shadow-xl rounded-2xl p-6">
        <h2 class="text-2xl font-bold text-blue-800 mb-4 text-center">Создание слотов</h2>
        
        <div v-if="selectedDoctorId">
          <!-- Выбор даты -->
          <div class="mb-6">
            <label class="block text-gray-700 font-medium mb-1">Дата приема</label>
            <input 
              type="date" 
              v-model="slotDate" 
              :min="minDate"
              class="w-full border rounded-lg px-4 py-2"
            >
          </div>
          
          <!-- Выбор времени начала -->
          <div class="mb-6">
            <label class="block text-gray-700 font-medium mb-1">Время начала</label>
            <input 
              type="time" 
              v-model="slotStartTime" 
              class="w-full border rounded-lg px-4 py-2"
            >
          </div>
          
          <!-- Количество сеансов -->
          <div class="mb-6">
            <label class="block text-gray-700 font-medium mb-1">Количество сеансов (1-10)</label>
            <input 
              type="number" 
              v-model.number="numberOfAppointments" 
              min="1" 
              max="10" 
              class="w-full border rounded-lg px-4 py-2"
            >
          </div>
          
          <!-- Кнопка создания -->
          <div class="mt-6 text-center">
            <button
              @click="createSlots"
              :disabled="!isFormValid"
              class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg disabled:bg-gray-400"
            >
              Создать слоты
            </button>
          </div>
          
          <!-- Предпросмотр слотов -->
          <div v-if="previewSlots.length > 0" class="mt-8">
            <h3 class="text-xl font-semibold mb-4">Будут созданы следующие слоты:</h3>
            <div class="grid grid-cols-3 gap-2">
              <div v-for="(slot, index) in previewSlots" :key="index" class="border p-2 rounded bg-gray-50">
                <p class="font-medium">Слот {{ index + 1 }}</p>
                <p>{{ formatDateTime(slot.startDateTime) }}</p>
                <p>{{ formatDateTime(slot.endDateTime) }}</p>
              </div>
            </div>
          </div>
        </div>
        
        <div v-else class="text-center py-8 text-gray-500">
          <p>Выберите врача для создания слотов</p>
        </div>
      </div>
    </div>
    
    <!-- Уведомление об успехе -->
    <div v-if="showSuccess" class="fixed inset-0 bg-gray-900 bg-opacity-50 flex justify-center items-center z-50">
      <div class="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full">
        <h3 class="text-xl font-bold text-center text-green-600 mb-4">Успешно!</h3>
        <p class="text-center mb-4">Создано {{ createdSlotsCount }} слотов для приема</p>
        <div class="text-center">
          <button @click="showSuccess = false" class="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg">
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, watch } from 'vue';
import axios from 'axios';

export default {
  setup() {
    const doctors = ref([]);
    const selectedDoctorId = ref('');
    const doctorSlots = ref([]);
    const slotDate = ref('');
    const slotStartTime = ref('09:00');
    const numberOfAppointments = ref(5);
    const createdSlotsCount = ref(0);
    const showSuccess = ref(false);
    
    // Получаем текущую дату для ограничения выбора
    const today = new Date();
    const minDate = ref(today.toISOString().split('T')[0]);
    
    // Загружаем список врачей при монтировании компонента
    const fetchDoctors = async () => {
      try {
        const token = localStorage.getItem('token');
        console.log(token);
        const response = await axios.get('https://localhost:7098/api/doctor/GetDoctors', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        doctors.value = response.data;
        console.log(doctors.value);
      } catch (error) {
        console.error('Ошибка при загрузке врачей:', error);
      }
    };
    
    // Загружаем слоты для выбранного врача
    const fetchDoctorSlots = async (doctorId) => {
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get(`https://localhost:7098/schedule/api/doctor-slots/${doctorId}`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        doctorSlots.value = response.data;
        
      } catch (error) {
        console.error('Ошибка при загрузке слотов врача:', error);
      }
    };
    
    // Создаем слоты для врача
    const createSlots = async () => {
      try {
        console.log( selectedDoctorId.value);
        const startDateTime = new Date(`${slotDate.value}T${slotStartTime.value}`);
        const request = {
          doctorId: selectedDoctorId.value,
          startDateTime: startDateTime.toISOString(),
          numberOfAppointments: numberOfAppointments.value
        };
        console.log("sadsafsaf",request);
        const response = await axios.post('https://localhost:7098/api/schedule/create-slots', request, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        
        createdSlotsCount.value = response.data.count;
        showSuccess.value = true;
        
        // Обновляем список слотов
        await fetchDoctorSlots(selectedDoctorId.value);
      } catch (error) {
        console.error('Ошибка при создании слотов:', error);
        alert('Произошла ошибка при создании слотов');
      }
    };
    
    // Форматирование даты и времени
    const formatDate = (dateString) => {
      return new Date(dateString).toLocaleDateString('ru-RU');
    };
    
    const formatTime = (dateString) => {
      return new Date(dateString).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
    };
    
    const formatDateTime = (dateString) => {
      const date = new Date(dateString);
      return `${date.toLocaleDateString('ru-RU')} ${date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}`;
    };
    
    // Предпросмотр слотов
    const previewSlots = computed(() => {
      if (!isFormValid.value) return [];
      
      const slots = [];
      const startDateTime = new Date(`${slotDate.value}T${slotStartTime.value}`);
      const appointmentDuration = 20 * 60 * 1000; // 20 минут в мс
      const breakBetween = 10 * 60 * 1000; // 10 минут в мс
      let currentStart = startDateTime.getTime();
      
      for (let i = 0; i < numberOfAppointments.value; i++) {
        const start = new Date(currentStart);
        const end = new Date(currentStart + appointmentDuration);
        
        slots.push({
          startDateTime: start.toISOString(),
          endDateTime: end.toISOString()
        });
        
        currentStart += appointmentDuration + breakBetween;
      }
      
      return slots;
    });
    
    // Валидация формы
    const isFormValid = computed(() => {
      return selectedDoctorId.value && 
             slotDate.value && 
             slotStartTime.value && 
             numberOfAppointments.value >= 1 && 
             numberOfAppointments.value <= 10;
    });
    
    // Следим за изменением выбранного врача
    watch(selectedDoctorId, (newVal) => {
      if (newVal) {
        fetchDoctorSlots(newVal);
      } else {
        doctorSlots.value = [];
      }
    });
    
    // Инициализация
    fetchDoctors();
    
    return {
      doctors,
      selectedDoctorId,
      doctorSlots,
      slotDate,
      slotStartTime,
      numberOfAppointments,
      minDate,
      previewSlots,
      isFormValid,
      createdSlotsCount,
      showSuccess,
      createSlots,
      formatDate,
      formatTime,
      formatDateTime
    };
  }
};
</script>

<style scoped>
</style>