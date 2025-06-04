<template>
  <div class="space-y-6">
    <!-- Заголовок -->
    <h1 class="text-3xl font-bold text-blue-900">Добро пожаловать, доктор!</h1>

    <!-- Карточки сводки -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Мои пациенты</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.patients }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Сегодняшние приёмы</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.todaysAppointments }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Ожидающие анализы</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.pendingLabs }}</p>
      </div>
    </div>

    <!-- Список приёмов -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold text-blue-800">Ближайшие приёмы</h2>
        <div class="flex space-x-2">
          <select v-model="sortOrder" class="border rounded p-2 text-sm">
            <option value="asc">По возрастанию</option>
            <option value="desc">По убыванию</option>
          </select>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Поиск по пациенту..."
            class="border rounded p-2 text-sm"
          />
        </div>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Пациент</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Дата и время</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Статус</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="appt in filteredAppointments" :key="appt.id">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div>
                    <div class="font-medium text-blue-900">
                      {{ appt.patient.surname }} {{ appt.patient.name }} {{ appt.patient.patronymic }}
                    </div>
                    <div class="text-sm text-gray-500">Полис: {{ appt.patient.policy }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(appt.slot.startDateTime) }} {{ formatTime(appt.slot.startDateTime) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="statusClasses(appt.status)" class="px-2 py-1 text-xs rounded-full">
                  {{ getAppointmentStatus(appt.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <button
                  @click="openCard(appt)"
                  class="text-blue-600 hover:text-blue-900 mr-3"
                >
                  Открыть карту
                </button>
              </td>
            </tr>
            <tr v-if="filteredAppointments.length === 0">
              <td colspan="4" class="px-6 py-4 text-center text-sm text-gray-500">
                Нет приёмов для отображения
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Модальное окно карточки приема -->
    <div
      v-if="showModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 p-4 overflow-auto"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg w-full max-w-4xl max-h-screen flex flex-col shadow-xl overflow-auto">
        <!-- Шапка модального окна -->
        <div class="flex justify-between items-center border-b p-4 bg-white">
          <h2 class="text-2xl font-semibold text-blue-900">
            Карточка приёма: {{ selectedAppointment.patient.surname }} {{ selectedAppointment.patient.name }}
          </h2>
          <button
            class="text-gray-500 hover:text-gray-700 text-2xl"
            @click="closeModal"
          >
            &times;
          </button>
        </div>

        <!-- Основной контент с прокруткой -->
        <div class="overflow-y-auto flex-1 p-6">
          <div class="space-y-6">
            <!-- Информация о пациенте -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Пациент</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p><span class="font-medium">ФИО:</span> {{ selectedAppointment.patient.surname }} {{ selectedAppointment.patient.name }} {{ selectedAppointment.patient.patronymic }}</p>
                  <p><span class="font-medium">Дата рождения:</span> {{ formatDate(selectedAppointment.patient.dateOfBirth) }}</p>
                </div>
                <div>
                  <p><span class="font-medium">Полис:</span> {{ selectedAppointment.patient.policy }}</p>
                  <p><span class="font-medium">Дата приёма:</span> {{ formatDateTime(selectedAppointment.slot.startDateTime) }}</p>
                </div>
              </div>
            </div>

            <!-- Форма заполнения заметок -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Заметки врача</h3>
              
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Явка</label>
                <textarea
                  v-model="notes.turnout"
                  class="w-full border rounded p-2 min-h-[50px]"
                  placeholder="Явка пациента..."
                ></textarea>
              </div>
              <!-- Анамнез -->
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Анамнез</label>
                <textarea
                  v-model="notes.anamnesis"
                  class="w-full border rounded p-2 min-h-[100px]"
                  placeholder="Введите анамнез пациента..."
                ></textarea>
              </div>

              <!-- Жалобы -->
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Жалобы</label>
                <textarea
                  v-model="notes.complaints"
                  class="w-full border rounded p-2 min-h-[100px]"
                  placeholder="Опишите жалобы пациента..."
                ></textarea>
              </div>

              <!-- Общее состояние -->
              <div>
                <h4 class="font-semibold mb-1 text-blue-700">Общее состояние</h4>
                <textarea
                  v-model="notes.generalConditions"
                  class="w-full border rounded p-2 min-h-[100px]"
                  placeholder="Опишите общее состояние пациента"
                ></textarea>
              </div>

              <!-- Физика -->
              <div>
                <h4 class="font-semibold mb-2 text-blue-700">Физика</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium mb-1">Вес (кг)</label>
                    <input
                      type="number"
                      v-model.number="notes.weight"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Рост (см)</label>
                    <input
                      type="number"
                      v-model.number="notes.height"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Давление (систолическое)</label>
                    <input
                      type="number"
                      v-model.number="notes.bloodPressureSystolic"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Давление (диастолическое)</label>
                    <input
                      type="number"
                      v-model.number="notes.bloodPressureDiastolic"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Температура (°C)</label>
                    <input
                      type="number"
                      v-model.number="notes.temperature"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Состояние</label>
                    <input
                      type="text"
                      v-model="notes.state"
                      class="w-full border rounded p-2"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Телосложение</label>
                    <input
                      v-model="notes.physique"
                      class="w-full border rounded p-2"
                      placeholder="Телосложение пациента..."
                    ></input>
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Кожа</label>
                    <input
                      type="text"
                      v-model.number="notes.skin"
                      class="w-full border rounded p-2"
                    />
                  </div>
                </div>
              </div>

              <!-- Лечение -->
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Лечение</label>
                <textarea
                  v-model="notes.treatment"
                  class="w-full border rounded p-2 min-h-[100px]"
                  placeholder="Назначенное лечение..."
                ></textarea>
              </div>

              <!-- Рекомендации -->
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Рекомендации</label>
                <textarea
                  v-model="notes.recommendations"
                  class="w-full border rounded p-2 min-h-[100px]"
                  placeholder="Рекомендации пациенту..."
                ></textarea>
              </div>
            </div>

            <!-- Назначение анализов -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Назначить анализы</h3>
              <div class="space-y-2 max-h-40 overflow-auto border p-2 rounded">
                <div v-for="test in availableTests" :key="test.testType" class="flex items-center">
                  <input
                    type="checkbox"
                    :value="test.testType"
                    v-model="selectedTests"
                    :disabled="isTestAssigned(test.testType)"
                    class="mr-2"
                  />
                  <label :for="`test-${test.testType}`" class="text-gray-700">
                    {{ test.testName }}
                    <span v-if="isTestAssigned(test.testType)" class="text-green-600 text-xs ml-1">(назначен)</span>
                  </label>
                </div>
              </div>
              <button
                @click="assignTests"
                class="mt-3 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
                :disabled="selectedTests.length === 0"
              >
                Назначить анализы
              </button>
            </div>
            
            <!-- Результаты анализов -->
            <div v-if="hasTestResults" class="mt-6 space-y-6">
              <h3 class="text-lg font-semibold text-blue-800">Результаты анализов</h3>

              <!-- Общий анализ крови -->
              <div v-if="hasTestWithResults('CBC')" class="border p-4 rounded shadow">
                <div class="flex justify-between items-center mb-2">
                  <h4 class="text-blue-700 font-semibold">Общий анализ крови</h4>
                  <span class="text-sm" :class="getTestStatusClass('CBC')">
                    {{ getTestStatus('CBC') }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Гемоглобин (г/л)</label>
                    <input :value="getTestResult('CBC', 'hemoglobin')" class="input" type="number" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Гематокрит (%)</label>
                    <input :value="getTestResult('CBC', 'hematocrit')" class="input" type="number" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Лейкоциты (×10⁹/л)</label>
                    <input :value="getTestResult('CBC', 'whiteBloodCells')" class="input" type="number" step="0.01" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Эритроциты (×10¹²/л)</label>
                    <input :value="getTestResult('CBC', 'redBloodCells')" class="input" type="number" step="0.01" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Тромбоциты (×10⁹/л)</label>
                    <input :value="getTestResult('CBC', 'platelets')" class="input" type="number" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">MCH (пг)</label>
                    <input :value="getTestResult('CBC', 'mch')" class="input" type="number" step="0.1" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">MCV (фл)</label>
                    <input :value="getTestResult('CBC', 'mcv')" class="input" type="number" step="0.1" readonly />
                  </div>
                </div>
                <div v-if="getTestComment('CBC')" class="mt-3 text-sm text-gray-600">
                  <p class="font-medium">Комментарий лаборанта:</p>
                  <p>{{ getTestComment('CBC') }}</p>
                </div>
              </div>

              <!-- Коагулограмма -->
              <div v-if="hasTestWithResults('Coagulogram')" class="border p-4 rounded shadow">
                <div class="flex justify-between items-center mb-2">
                  <h4 class="text-blue-700 font-semibold">Коагулограмма</h4>
                  <span class="text-sm" :class="getTestStatusClass('Coagulogram')">
                    {{ getTestStatus('Coagulogram') }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">PT (сек)</label>
                    <input :value="getTestResult('Coagulogram', 'pt')" class="input" type="number" step="0.1" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">INR</label>
                    <input :value="getTestResult('Coagulogram', 'inr')" class="input" type="number" step="0.01" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">APTT (сек)</label>
                    <input :value="getTestResult('Coagulogram', 'aptt')" class="input" type="number" step="0.1" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Фибриноген (г/л)</label>
                    <input :value="getTestResult('Coagulogram', 'fibrinogen')" class="input" type="number" step="0.01" readonly />
                  </div>
                </div>
                <div v-if="getTestComment('Coagulogram')" class="mt-3 text-sm text-gray-600">
                  <p class="font-medium">Комментарий лаборанта:</p>
                  <p>{{ getTestComment('Coagulogram') }}</p>
                </div>
              </div>

              <!-- Факторы свертывания -->
              <div v-if="hasTestWithResults('FactorAndVWF')" class="border p-4 rounded shadow">
                <div class="flex justify-between items-center mb-2">
                  <h4 class="text-blue-700 font-semibold">Факторы свертывания и VWF</h4>
                  <span class="text-sm" :class="getTestStatusClass('FactorAndVWF')">
                    {{ getTestStatus('FactorAndVWF') }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Фактор VIII (%)</label>
                    <input :value="getTestResult('FactorAndVWF', 'factorVIII')" class="input" type="number" step="0.1" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Фактор IX (%)</label>
                    <input :value="getTestResult('FactorAndVWF', 'factorIX')" class="input" type="number" step="0.1" readonly />
                  </div>
                  <div>
                    <label class="block text-sm text-gray-600 mb-1">Активность VWF (%)</label>
                    <input :value="getTestResult('FactorAndVWF', 'vwfActivity')" class="input" type="number" step="0.1" readonly />
                  </div>
                </div>
                <div v-if="getTestComment('FactorAndVWF')" class="mt-3 text-sm text-gray-600">
                  <p class="font-medium">Комментарий лаборанта:</p>
                  <p>{{ getTestComment('FactorAndVWF') }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Кнопки сохранения -->
        <div class="border-t p-4 bg-white flex justify-between">
          <button
            @click="closeModal"
            class="px-6 py-2 border rounded-lg text-gray-700 hover:bg-gray-100 transition"
          >
            Отмена
          </button>
          <div>
            <button
              @click="saveNotes(false)"
              class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition mr-2"
            >
              Сохранить
            </button>
            <button
              @click="saveNotes(true)"
              class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition"
            >
              Сохранить и закрыть
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'DoctorHomePage',
  data() {
    return {
      stats: {
        patients: 0,
        todaysAppointments: 0,
        pendingLabs: 0,
      },
      doctorId: null,
      upcomingAppointments: [],
      searchQuery: '',
      sortOrder: 'asc',
      showModal: false,
      selectedAppointment: {
        id: null,
        patient: {},
        slot: {},
        notes: {},
        tests: []
      },
      notes: {},
      availableTests: [
        { testType: "CBC", testName: "Общий анализ крови" },
        { testType: "Coagulogram", testName: "Коагулограмма" },
        { testType: "FactorAndVWF", testName: "Факторы свертывания и VWF" }
      ],
      selectedTests: [],
    };
  },

  computed: {
    filteredAppointments() {
      let filtered = this.upcomingAppointments;

      // Фильтрация по поисковому запросу
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(appt => 
          appt.patient.surname.toLowerCase().includes(query) ||
          appt.patient.name.toLowerCase().includes(query) ||
          appt.patient.patronymic.toLowerCase().includes(query) ||
          appt.patient.policy.toLowerCase().includes(query)
        );
      }

      // Сортировка по времени
      filtered.sort((a, b) => {
        const dateA = new Date(a.slot.startDateTime);
        const dateB = new Date(b.slot.startDateTime);
        return this.sortOrder === 'asc' ? dateA - dateB : dateB - dateA;
      });

      return filtered;
    },

    hasTestResults() {
      return this.selectedAppointment?.tests?.some(test => 
        (test.cbcDetails || test.coagulogramDetails || test.factorAndVwfDetails)
      );
    }
  },

  methods: {
    statusClasses(status) {
      return {
        'bg-gray-100 text-gray-800': status === 'Scheduled',
        'bg-green-100 text-green-800': status === 'Completed',
        'bg-red-100 text-red-800': status === 'Cancelled'
      };
    },

    getAppointmentStatus(status) {
      const statuses = {
        'Scheduled': 'Запланирован',
        'Completed': 'Завершен',
        'Cancelled': 'Отменен'
      };
      return statuses[status] || status;
    },

    // Проверяет, назначен ли уже тест
    isTestAssigned(testType) {
      return this.selectedAppointment?.tests?.some(t => t.testType === testType);
    },

    // Проверяет наличие результатов для конкретного типа теста
    hasTestWithResults(testType) {
      const test = this.selectedAppointment?.tests?.find(t => t.testType === testType);
      if (!test) return false;
      
      switch(testType) {
        case 'CBC': return !!test.cbcDetails;
        case 'Coagulogram': return !!test.coagulogramDetails;
        case 'FactorAndVWF': return !!test.factorAndVwfDetails;
        default: return false;
      }
    },

    // Получает результат конкретного теста
    getTestResult(testType, field) {
      const test = this.selectedAppointment?.tests?.find(t => t.testType === testType);
      if (!test) return '';
      
      switch(testType) {
        case 'CBC': return test.cbcDetails?.[field] || '';
        case 'Coagulogram': return test.coagulogramDetails?.[field] || '';
        case 'FactorAndVWF': return test.factorAndVwfDetails?.[field] || '';
        default: return '';
      }
    },

    // Получает статус теста
    getTestStatus(testType) {
      const test = this.selectedAppointment?.tests?.find(t => t.testType === testType);
      if (!test) return 'Не назначен';
      
      switch(test.status) {
        case 'Pending': return 'Ожидает выполнения';
        case 'InProgress': return 'В процессе';
        case 'Completed': return 'Завершен';
        default: return test.status;
      }
    },

    // Получает класс для статуса теста
    getTestStatusClass(testType) {
      const test = this.selectedAppointment?.tests?.find(t => t.testType === testType);
      if (!test) return '';
      
      switch(test.status) {
        case 'Pending': return 'text-yellow-600';
        case 'InProgress': return 'text-blue-600';
        case 'Completed': return 'text-green-600';
        default: return '';
      }
    },

    // Получает комментарий к тесту
    getTestComment(testType) {
      const test = this.selectedAppointment?.tests?.find(t => t.testType === testType);
      return test?.result || '';
    },

    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString();
    },

    formatTime(timeString) {
      if (!timeString) return '';
      const date = new Date(timeString);
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    },

    formatDateTime(datetime) {
      if (!datetime) return '';
      const date = new Date(datetime);
      return date.toLocaleString();
    },

    openCard(appt) {
      this.selectedAppointment = JSON.parse(JSON.stringify(appt));
      this.notes = appt.notes ? {...appt.notes} : {};
      this.selectedTests = appt.tests?.map(t => t.testType) || [];
      this.showModal = true;
    },

    closeModal() {
      this.showModal = false;
      this.selectedAppointment = {
        id: null,
        patient: {},
        slot: {},
        notes: {},
        tests: []
      };
      this.notes = {};
      this.selectedTests = [];
    },

    async getDoctorId() {
      try {
        const response = await axios.get(
          `https://localhost:7098/api/doctor/getDoctorByUserId/${localStorage.getItem('userId')}`, 
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );
        this.doctorId = response.data.id;
      } catch (error) {
        console.error('Ошибка при получении ID врача:', error);
      }
    },

    async getDoctorReceptions() {
      if (!this.doctorId) return;
      
      try {
        const response = await axios.get(
          `https://localhost:7098/api/reception/doctor-receptions/${this.doctorId}`, 
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );
        
        this.upcomingAppointments = response.data || [];
        
        // Статистика
        const today = new Date().toISOString().split('T')[0];
        this.stats.todaysAppointments = this.upcomingAppointments.filter(appt =>
          appt.slot?.startDateTime?.startsWith(today)
        ).length;
        
        this.stats.patients = new Set(this.upcomingAppointments.map(appt => appt.patientId)).size;
        
        this.stats.pendingLabs = this.upcomingAppointments.reduce((acc, appt) => {
          return acc + (appt.tests?.filter(t => t.status !== 'Completed').length || 0);
        }, 0);
      } catch (error) {
        console.error('Ошибка при загрузке приёмов:', error);
      }
    },

    async saveNotes(closeAfterSave) {
      if (!this.selectedAppointment?.id || !this.notes?.id) return;

      try {
        const updateNotesDto = {
          anamnesis: this.notes.anamnesis,
          complaints: this.notes.complaints,
          generalConditions: this.notes.generalConditions,
          physique: this.notes.physique,
          weight: this.notes.weight, 
          height: this.notes.height,
          bloodPressureSystolic: this.notes.bloodPressureSystolic,
          bloodPressureDiastolic: this.notes.bloodPressureDiastolic,
          temperature: this.notes.temperature,
          state: this.notes.state,
          skin: this.notes.skin,
          examination: this.notes.examination,
          treatment: this.notes.treatment,
          recommendations: this.notes.recommendations,
          turnout: this.notes.turnout || ''
        };
        console.log('Обновляемые заметки:', this.notes.physique,this.notes.turnout);
        await axios.put(
          `https://localhost:7098/api/notes/updateNote/${this.notes.id}`, 
          updateNotesDto, 
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        // Обновляем данные в интерфейсе
        const index = this.upcomingAppointments.findIndex(a => a.id === this.selectedAppointment.id);
        if (index !== -1) {
          this.upcomingAppointments[index].notes = {...this.notes};
        }

        alert('Записи успешно сохранены');
        if (closeAfterSave) this.closeModal();
      } catch (error) {
        console.error('Ошибка при сохранении заметок:', error);
        alert('Не удалось сохранить заметки');
      }
    },

    async assignTests() {
      if (!this.selectedTests.length || !this.selectedAppointment?.id) return;
      
      try {
        const payload = {
          patientId: this.selectedAppointment.patientId,
          tests: this.selectedTests.map(testType => {
            const test = this.availableTests.find(t => t.testType === testType);
            return {
              testType: test.testType,
              testName: test.testName
            };
          })
        };

        await axios.put(
          `https://localhost:7098/api/reception/assign-tests/${this.selectedAppointment.id}`, 
          payload, 
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        // Обновляем список тестов в интерфейсе
        const newTests = this.selectedTests.filter(testType => 
          !this.selectedAppointment.tests.some(t => t.testType === testType)
        ).map(testType => ({
          testType,
          status: 'Pending'
        }));

        this.selectedAppointment.tests = [...this.selectedAppointment.tests, ...newTests];
        
        // Обновляем статистику
        this.stats.pendingLabs += newTests.length;

        alert("Анализы успешно назначены");
      } catch (error) {
        console.error("Ошибка при назначении анализов", error);
        alert("Не удалось назначить анализы");
      }
    },

    async removeTest(testType) {
      if (!this.selectedAppointment?.id) return;

      try {
        await axios.delete(
          `https://localhost:7098/api/tests/remove-test/${this.selectedAppointment.id}/${testType}`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        // Обновляем интерфейс
        const testIndex = this.selectedAppointment.tests.findIndex(t => t.testType === testType);
        if (testIndex !== -1) {
          // Уменьшаем счетчик pendingLabs только если тест не был завершен
          if (this.selectedAppointment.tests[testIndex].status !== 'Completed') {
            this.stats.pendingLabs--;
          }
          this.selectedAppointment.tests.splice(testIndex, 1);
        }

        this.selectedTests = this.selectedTests.filter(t => t !== testType);

        alert(`Анализ ${testType} удалён`);
      } catch (error) {
        console.error("Ошибка при удалении анализа", error);
        alert("Не удалось удалить анализ");
      }
    }
  },

  async mounted() {
    await this.getDoctorId();
    if (this.doctorId) {
      await this.getDoctorReceptions();
    }
  }
};
</script>

<style>
.overflow-y-auto {
  overflow-y: scroll !important;
  -webkit-overflow-scrolling: touch;
}

.overflow-y-auto::-webkit-scrollbar {
  width: 10px;
  height: 10px;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 10px;
  border: 2px solid transparent;
  background-clip: padding-box;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

</style>