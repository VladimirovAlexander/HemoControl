<template>
  <div class="space-y-6">
    <!-- Заголовок -->
    <h1 class="text-3xl font-bold text-blue-900">Мои приёмы</h1>

    <!-- Карточки сводки -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Всего приёмов</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.totalAppointments }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Сегодняшние приёмы</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.todaysAppointments }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Завершенные</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.completedAppointments }}</p>
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
            placeholder="Поиск по врачу..."
            class="border rounded p-2 text-sm"
          />
        </div>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Врач</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Специальность</th>
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
                      {{ appt.doctor.lastName }} {{ appt.doctor.firstName }}
                    </div>
                    <div class="text-sm text-gray-500">Кабинет: {{ appt.doctor.officeNumber }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ appt.doctor.specialty }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(appt.slotInfo.startDateTime) }} {{ formatTime(appt.slotInfo.startDateTime) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="statusClasses(appt.status)" class="px-2 py-1 text-xs rounded-full">
                  {{ getAppointmentStatus(appt.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <button
                  @click="openDetails(appt)"
                  class="text-blue-600 hover:text-blue-900 mr-3"
                >
                  Подробнее
                </button>
                <button
                  v-if="appt.status === 'Scheduled'"
                  @click="cancelAppointment(appt.id)"
                  class="text-red-600 hover:text-red-900"
                >
                  Отменить
                </button>
              </td>
            </tr>
            <tr v-if="filteredAppointments.length === 0">
              <td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">
                Нет приёмов для отображения
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Модальное окно с подробной информацией -->
    <div
      v-if="showModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 p-4 overflow-auto"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg w-full max-w-4xl max-h-screen flex flex-col shadow-xl overflow-auto">
        <!-- Шапка модального окна -->
        <div class="flex justify-between items-center border-b p-4 bg-white">
          <h2 class="text-2xl font-semibold text-blue-900">
            Карточка приёма: {{ selectedAppointment.doctor.lastName }} {{ selectedAppointment.doctor.firstName }}
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
            <!-- Информация о приеме -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Информация о приёме</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p><span class="font-medium">Дата:</span> {{ formatDate(selectedAppointment.slotInfo.startDateTime) }}</p>
                  <p><span class="font-medium">Время:</span> {{ formatTime(selectedAppointment.slotInfo.startDateTime) }} - {{ formatTime(selectedAppointment.slotInfo.endDateTime) }}</p>
                  <p><span class="font-medium">Статус:</span>
                    <span :class="statusClasses(selectedAppointment.status)" class="px-2 py-1 text-xs rounded-full">
                      {{ getAppointmentStatus(selectedAppointment.status) }}
                    </span>
                  </p>
                </div>
                <div>
                  <p><span class="font-medium">Кабинет:</span> {{ selectedAppointment.doctor.officeNumber }}</p>
                  <p><span class="font-medium">Специальность:</span> {{ selectedAppointment.doctor.specialty }}</p>
                </div>
              </div>
            </div>

            <!-- Информация о враче -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Врач</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p><span class="font-medium">ФИО:</span> {{ selectedAppointment.doctor.lastName }} {{ selectedAppointment.doctor.firstName }} {{ selectedAppointment.doctor.patronymic }}</p>
                </div>
                <div>
                  <p><span class="font-medium">Контактный телефон:</span> {{ selectedAppointment.doctor.phoneNumber || 'Не указан' }}</p>
                </div>
              </div>
            </div>

            <!-- Заметки врача -->
            <div v-if="selectedAppointment.notes">
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Заметки врача</h3>
              
              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Явка</label>
                <textarea
                  v-model="selectedAppointment.notes.turnout"
                  class="w-full border rounded p-2 min-h-[50px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Анамнез</label>
                <textarea
                  v-model="selectedAppointment.notes.anamnesis"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Жалобы</label>
                <textarea
                  v-model="selectedAppointment.notes.complaints"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Общее состояние</label>
                <textarea
                  v-model="selectedAppointment.notes.generalConditions"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div>
                <h4 class="font-semibold mb-2 text-blue-700">Физика</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium mb-1">Вес (кг)</label>
                    <input
                      type="number"
                      v-model.number="selectedAppointment.notes.weight"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Рост (см)</label>
                    <input
                      type="number"
                      v-model.number="selectedAppointment.notes.height"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Давление (систолическое)</label>
                    <input
                      type="number"
                      v-model.number="selectedAppointment.notes.bloodPressureSystolic"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Давление (диастолическое)</label>
                    <input
                      type="number"
                      v-model.number="selectedAppointment.notes.bloodPressureDiastolic"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Температура (°C)</label>
                    <input
                      type="number"
                      v-model.number="selectedAppointment.notes.temperature"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Состояние</label>
                    <input
                      type="text"
                      v-model="selectedAppointment.notes.state"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Телосложение</label>
                    <input
                      v-model="selectedAppointment.notes.physique"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Кожа</label>
                    <input
                      type="text"
                      v-model.number="selectedAppointment.notes.skin"
                      class="w-full border rounded p-2 bg-gray-50"
                      readonly
                    />
                  </div>
                </div>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Обследование</label>
                <textarea
                  v-model="selectedAppointment.notes.examination"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Лечение</label>
                <textarea
                  v-model="selectedAppointment.notes.treatment"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>

              <div class="mb-4">
                <label class="block text-sm font-medium mb-1">Рекомендации</label>
                <textarea
                  v-model="selectedAppointment.notes.recommendations"
                  class="w-full border rounded p-2 min-h-[100px] bg-gray-50"
                  readonly
                ></textarea>
              </div>
            </div>
            <div v-else>
              <p class="text-gray-500">Заметки врача отсутствуют</p>
            </div>

            <!-- Назначенные анализы -->
            <div v-if="selectedAppointment.tests?.length > 0">
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Назначенные анализы</h3>
              <div class="space-y-6">
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
                      <input :value="getTestResult('CBC', 'Hemoglobin')" class="input" type="number" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Гематокрит (%)</label>
                      <input :value="getTestResult('CBC', 'Hematocrit')" class="input" type="number" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Лейкоциты (×10⁹/л)</label>
                      <input :value="getTestResult('CBC', 'WBC')" class="input" type="number" step="0.01" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Эритроциты (×10¹²/л)</label>
                      <input :value="getTestResult('CBC', 'RBC')" class="input" type="number" step="0.01" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Тромбоциты (×10⁹/л)</label>
                      <input :value="getTestResult('CBC', 'Platelets')" class="input" type="number" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">MCH (пг)</label>
                      <input :value="getTestResult('CBC', 'MCH')" class="input" type="number" step="0.1" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">MCV (фл)</label>
                      <input :value="getTestResult('CBC', 'MCV')" class="input" type="number" step="0.1" readonly />
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
                      <input :value="getTestResult('Coagulogram', 'PT')" class="input" type="number" step="0.1" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">INR</label>
                      <input :value="getTestResult('Coagulogram', 'INR')" class="input" type="number" step="0.01" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">APTT (сек)</label>
                      <input :value="getTestResult('Coagulogram', 'APTT')" class="input" type="number" step="0.1" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Фибриноген (г/л)</label>
                      <input :value="getTestResult('Coagulogram', 'Fibrinogen')" class="input" type="number" step="0.01" readonly />
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
                      <input :value="getTestResult('FactorAndVWF', 'FactorVIII')" class="input" type="number" step="0.1" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Фактор IX (%)</label>
                      <input :value="getTestResult('FactorAndVWF', 'FactorIX')" class="input" type="number" step="0.1" readonly />
                    </div>
                    <div>
                      <label class="block text-sm text-gray-600 mb-1">Активность VWF (%)</label>
                      <input :value="getTestResult('FactorAndVWF', 'VWFActivity')" class="input" type="number" step="0.1" readonly />
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
        </div>

        <!-- Кнопки -->
        <div class="border-t p-4 bg-white flex justify-end">
          <button
            @click="closeModal"
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'PatientReceptionsPage',
  data() {
    return {
      stats: {
        totalAppointments: 0,
        todaysAppointments: 0,
        completedAppointments: 0,
      },
      receptions: [],
      loading: true,
      searchQuery: '',
      sortOrder: 'asc',
      showModal: false,
      selectedAppointment: {
        id: null,
        doctor: {},
        slotInfo: {},
        notes: {},
        tests: []
      },
    };
  },

  computed: {
    filteredAppointments() {
      let filtered = this.receptions;

      // Фильтрация по поисковому запросу
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(appt => 
          appt.doctor.lastName.toLowerCase().includes(query) ||
          appt.doctor.firstName.toLowerCase().includes(query) ||
          appt.doctor.specialty.toLowerCase().includes(query)
        );
      }

      // Сортировка по времени
      filtered.sort((a, b) => {
        const dateA = new Date(a.slotInfo.startDateTime);
        const dateB = new Date(b.slotInfo.startDateTime);
        return this.sortOrder === 'asc' ? dateA - dateB : dateB - dateA;
      });

      return filtered;
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

    testStatusClass(status) {
      return {
        'text-yellow-600': status === 'Pending',
        'text-blue-600': status === 'InProgress',
        'text-green-600': status === 'Completed'
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

    getTestStatus(testType) {
      const test = this.selectedAppointment.tests?.find(t => t.testType === testType);
      if (!test) return 'Не назначен';
      
      const statuses = {
        'Pending': 'Ожидает выполнения',
        'InProgress': 'В процессе',
        'Completed': 'Завершен'
      };
      return statuses[test.status] || test.status;
    },

    getTestStatusClass(testType) {
      const test = this.selectedAppointment.tests?.find(t => t.testType === testType);
      if (!test) return '';
      
      return {
        'Pending': 'text-yellow-600',
        'InProgress': 'text-blue-600',
        'Completed': 'text-green-600'
      }[test.status] || '';
    },

    hasTestWithResults(testType) {
      const test = this.selectedAppointment.tests?.find(t => t.testType === testType);
      if (!test) return false;
      
      return test.parameters && Object.keys(test.parameters).length > 0;
    },

    getTestResult(testType, field) {
      const test = this.selectedAppointment.tests?.find(t => t.testType === testType);
      if (!test || !test.parameters) return '';
      
      return test.parameters[field] || '';
    },

    getTestComment(testType) {
      const test = this.selectedAppointment.tests?.find(t => t.testType === testType);
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

    openDetails(appt) {
      this.selectedAppointment = JSON.parse(JSON.stringify(appt));
      this.showModal = true;
    },

    closeModal() {
      this.showModal = false;
      this.selectedAppointment = {
        id: null,
        doctor: {},
        slotInfo: {},
        notes: {},
        tests: []
      };
    },

    async cancelAppointment(id) {
      if (!confirm('Вы уверены, что хотите отменить приём?')) return;
      
      try {
        await axios.delete(`https://localhost:7098/api/reception/${id}`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        
        this.receptions = this.receptions.map(r => 
          r.id === id ? { ...r, status: 'Cancelled' } : r
        );
        
        alert('Приём успешно отменён');
      } catch (error) {
        console.error('Ошибка при отмене приёма:', error);
        alert('Не удалось отменить приём');
      }
    },

    async fetchReceptions() {
      this.loading = true;
      const patientId = localStorage.getItem('patientId');

      try {
        const response = await axios.get(
          `https://localhost:7098/api/reception/user-receptions/${patientId}`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        const receptionList = response.data || [];

        // Обогащаем данные о врачах и слотах
        const enrichedReceptions = await Promise.all(
          receptionList.map(async (reception) => {
            // Получаем доктора
            try {
              const doctorRes = await axios.get(
                `https://localhost:7098/api/doctor/${reception.doctorId}`,
                {
                  headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
                }
              );
              reception.doctor = doctorRes.data;
            } catch {
              reception.doctor = {};
            }

            // Получаем слот
            try {
              const slotRes = await axios.get(
                `https://localhost:7098/api/schedule/get-slot/${reception.slotId}`,
                {
                  headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
                }
              );
              reception.slotInfo = slotRes.data;
            } catch {
              reception.slotInfo = {};
            }

            // Получаем заметки
            if (reception.notesId) {
              try {
                const notesRes = await axios.get(
                  `https://localhost:7098/api/notes/${reception.notesId}`,
                  {
                    headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
                  }
                );
                reception.notes = notesRes.data;
              } catch {
                reception.notes = null;
              }
            }

            // Получаем анализы
            try {
              const testsRes = await axios.get(
                `https://localhost:7098/api/tests/reception/${reception.id}`,
                {
                  headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
                }
              );
              reception.tests = testsRes.data || [];
            } catch {
              reception.tests = [];
            }

            return reception;
          })
        );

        this.receptions = enrichedReceptions;
        
        // Обновляем статистику
        const today = new Date().toISOString().split('T')[0];
        this.stats.totalAppointments = this.receptions.length;
        this.stats.todaysAppointments = this.receptions.filter(appt =>
          appt.slotInfo?.startDateTime?.startsWith(today)
        ).length;
        this.stats.completedAppointments = this.receptions.filter(appt =>
          appt.status === 'Completed'
        ).length;
      } catch (error) {
        console.error('Ошибка при загрузке приёмов:', error);
      } finally {
        this.loading = false;
      }
    }
  },

  async mounted() {
    await this.fetchReceptions();
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