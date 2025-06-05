<template>
  <div class="space-y-6">
    <!-- Заголовок -->
    <h1 class="text-3xl font-bold text-blue-900">Мои анализы</h1>

    <!-- Карточки сводки -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Всего анализов</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.totalTests }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Завершенные</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.completedTests }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Ожидают результатов</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.pendingTests }}</p>
      </div>
    </div>

    <!-- Фильтры -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-4">
        <h2 class="text-xl font-semibold text-blue-800">История анализов</h2>
        <div class="flex flex-col md:flex-row gap-2">
          <select v-model="statusFilter" class="border rounded p-2 text-sm">
            <option value="all">Все статусы</option>
            <option value="Completed">Завершенные</option>
            <option value="InProgress">В процессе</option>
            <option value="Pending">Ожидают</option>
          </select>
          <select v-model="typeFilter" class="border rounded p-2 text-sm">
            <option value="all">Все типы</option>
            <option v-for="type in testTypes" :value="type" :key="type">{{ getTestTypeName(type) }}</option>
          </select>
          <input
            v-model="dateFilter"
            type="date"
            class="border rounded p-2 text-sm"
          />
        </div>
      </div>

      <!-- Список анализов -->
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Тип анализа</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Дата назначения</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Статус</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="test in filteredTests" :key="test.id">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="font-medium text-blue-900">
                  {{ getTestTypeName(test.testType) }}
                </div>
                <div v-if="test.receptionId" class="text-sm text-gray-500">
                  Назначено на приеме: {{ formatDate(getReceptionDate(test.receptionId)) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(test.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="testStatusClasses(test.status)" class="px-2 py-1 text-xs rounded-full">
                  {{ getTestStatus(test.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <button
                  @click="openTestDetails(test)"
                  class="text-blue-600 hover:text-blue-900"
                >
                  Просмотреть
                </button>
              </td>
            </tr>
            <tr v-if="filteredTests.length === 0">
              <td colspan="4" class="px-6 py-4 text-center text-sm text-gray-500">
                Нет анализов для отображения
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Модальное окно с результатами анализов -->
    <div
      v-if="showTestModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 p-4 overflow-auto"
      @click.self="closeTestModal"
    >
      <div class="bg-white rounded-lg w-full max-w-4xl max-h-screen flex flex-col shadow-xl overflow-auto">
        <!-- Шапка модального окна -->
        <div class="flex justify-between items-center border-b p-4 bg-white">
          <h2 class="text-2xl font-semibold text-blue-900">
            Результаты анализа: {{ getTestTypeName(selectedTest.testType) }}
          </h2>
          <button
            class="text-gray-500 hover:text-gray-700 text-2xl"
            @click="closeTestModal"
          >
            &times;
          </button>
        </div>

        <!-- Основной контент с прокруткой -->
        <div class="overflow-y-auto flex-1 p-6">
          <div class="space-y-6">
            <!-- Информация о анализе -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="bg-gray-50 p-4 rounded-lg">
                <h3 class="text-sm font-medium text-gray-500 mb-1">Дата назначения</h3>
                <p class="font-medium">{{ formatDate(selectedTest.createdAt) }}</p>
              </div>
              <div class="bg-gray-50 p-4 rounded-lg">
                <h3 class="text-sm font-medium text-gray-500 mb-1">Дата выполнения</h3>
                <p class="font-medium">{{ selectedTest.completedAt ? formatDate(selectedTest.completedAt) : 'Не выполнено' }}</p>
              </div>
              <div class="bg-gray-50 p-4 rounded-lg">
                <h3 class="text-sm font-medium text-gray-500 mb-1">Статус</h3>
                <p>
                  <span :class="testStatusClasses(selectedTest.status)" class="px-2 py-1 text-xs rounded-full">
                    {{ getTestStatus(selectedTest.status) }}
                  </span>
                </p>
              </div>
            </div>

            <!-- Результаты анализов -->
            <template v-if="selectedTest.parameters">
              <h3 class="text-xl font-semibold mb-4 text-blue-800">Результаты</h3>
              
              <!-- Общий анализ крови -->
              <div v-if="selectedTest.testType === 'CBC'" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Гемоглобин (г/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Hemoglobin || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">120-160</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Гематокрит (%)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Hematocrit || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">36-48</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Лейкоциты (×10⁹/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.WBC || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">4.0-9.0</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Эритроциты (×10¹²/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.RBC || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">3.9-5.5</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Тромбоциты (×10⁹/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Platelets || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">150-400</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">MCH (пг)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.MCH || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">27-31</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">MCV (фл)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.MCV || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">80-100</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Биохимический анализ крови -->
              <div v-else-if="selectedTest.testType === 'Biochemistry'" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Глюкоза (ммоль/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Glucose || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">3.3-5.5</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Общий белок (г/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.TotalProtein || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">65-85</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">АЛТ (Ед/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.ALT || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">0-41</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">АСТ (Ед/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.AST || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">0-37</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Креатинин (мкмоль/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Creatinine || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">53-115</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Мочевина (ммоль/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Urea || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">2.8-7.2</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Коагулограмма -->
              <div v-else-if="selectedTest.testType === 'Coagulogram'" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Протромбиновое время (сек)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.PT || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">11-16</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">INR</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.INR || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">0.8-1.2</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">АЧТВ (сек)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.APTT || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">25-37</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Фибриноген (г/л)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.Fibrinogen || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">2.0-4.0</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">D-димер (мкг/мл)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.DDimer || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">0-0.55</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Факторы свертывания -->
              <div v-else-if="selectedTest.testType === 'FactorAndVWF'" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Фактор VIII (%)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.FactorVIII || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">50-150</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Фактор IX (%)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.FactorIX || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">50-150</span>
                    </div>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Активность VWF (%)</label>
                    <div class="relative">
                      <input 
                        :value="selectedTest.parameters.VWFActivity || ''" 
                        class="w-full border rounded p-2 bg-gray-50 pr-10" 
                        readonly 
                      />
                      <span class="absolute right-3 top-2 text-sm text-gray-500">50-150</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Комментарий лаборанта -->
              <div v-if="selectedTest.result" class="mt-6">
                <h4 class="text-lg font-semibold mb-2 text-blue-700">Комментарий лаборанта</h4>
                <div class="bg-blue-50 p-4 rounded-lg">
                  <p class="whitespace-pre-wrap">{{ selectedTest.result }}</p>
                </div>
              </div>
            </template>
            <div v-else class="text-center py-8 text-gray-500">
              <p class="text-lg">
                {{ selectedTest.status === 'Completed' ? 'Результаты анализа отсутствуют' : 'Анализ еще не завершен' }}
              </p>
              <p class="text-sm mt-2">
                {{ selectedTest.status === 'Pending' ? 'Ожидайте выполнения анализа' : 
                   selectedTest.status === 'InProgress' ? 'Анализ в процессе выполнения' : 
                   'Результаты пока не доступны' }}
              </p>
            </div>
          </div>
        </div>

        <!-- Кнопки -->
        <div class="border-t p-4 bg-white flex justify-end">
          <button
            @click="closeTestModal"
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
  name: 'PatientLabResultsPage',
  data() {
    return {
      stats: {
        totalTests: 0,
        completedTests: 0,
        pendingTests: 0,
      },
      tests: [],
      receptions: [],
      loading: true,
      statusFilter: 'all',
      typeFilter: 'all',
      dateFilter: '',
      testTypes: ['CBC', 'Biochemistry', 'Coagulogram', 'FactorAndVWF', 'Hormones', 'Urinalysis'],
      showTestModal: false,
      selectedTest: {
        id: null,
        testType: '',
        status: '',
        parameters: null,
        result: '',
        createdAt: '',
        completedAt: null,
        receptionId: null
      },
    };
  },

  computed: {
    filteredTests() {
      let filtered = this.tests;

      if (this.statusFilter !== 'all') {
        filtered = filtered.filter(test => test.status === this.statusFilter);
      }

      if (this.typeFilter !== 'all') {
        filtered = filtered.filter(test => test.testType === this.typeFilter);
      }

      if (this.dateFilter) {
        const filterDate = new Date(this.dateFilter).toISOString().split('T')[0];
        filtered = filtered.filter(test => 
          new Date(test.createdAt).toISOString().split('T')[0] === filterDate
        );
      }

      return filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    }
  },

  methods: {
    testStatusClasses(status) {
      return {
        'bg-yellow-100 text-yellow-800': status === 'Pending',
        'bg-blue-100 text-blue-800': status === 'InProgress',
        'bg-green-100 text-green-800': status === 'Completed'
      };
    },

    getTestStatus(status) {
      const statuses = {
        'Pending': 'Ожидает выполнения',
        'InProgress': 'В процессе',
        'Completed': 'Завершен'
      };
      return statuses[status] || status;
    },

    getTestTypeName(type) {
      const names = {
        'CBC': 'Общий анализ крови',
        'Biochemistry': 'Биохимический анализ крови',
        'Coagulogram': 'Коагулограмма',
        'FactorAndVWF': 'Факторы свертывания и VWF',
        'Hormones': 'Анализ на гормоны',
        'Urinalysis': 'Общий анализ мочи'
      };
      return names[type] || type;
    },

    formatDate(dateString) {
      if (!dateString) return '';
      try {
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU');
      } catch {
        return dateString;
      }
    },

    getReceptionDate(receptionId) {
      const reception = this.receptions.find(r => r.id === receptionId);
      return reception?.slotInfo?.startDateTime || '';
    },

    openTestDetails(test) {
      this.selectedTest = {
        ...test,
        parameters: test.parameters || null,
        result: test.result || ''
      };
      this.showTestModal = true;
    },

    closeTestModal() {
      this.showTestModal = false;
      this.selectedTest = {
        id: null,
        testType: '',
        status: '',
        parameters: null,
        result: '',
        createdAt: '',
        completedAt: null,
        receptionId: null
      };
    },

    async fetchTests() {
      this.loading = true;
      const patientId = localStorage.getItem('patientId');

      try {
        // Получаем все анализы пациента
        const testsResponse = await axios.get(
          `https://localhost:7098/api/tests/patient/${patientId}`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        this.tests = testsResponse.data || [];

        // Получаем приемы для дополнительной информации
        await this.fetchReceptions();

        // Обновляем статистику
        this.stats.totalTests = this.tests.length;
        this.stats.completedTests = this.tests.filter(t => t.status === 'Completed').length;
        this.stats.pendingTests = this.tests.filter(t => t.status !== 'Completed').length;
      } catch (error) {
        console.error('Ошибка при загрузке анализов:', error);
      } finally {
        this.loading = false;
      }
    },

    async fetchReceptions() {
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

        // Обогащаем данные о слотах
        this.receptions = await Promise.all(
          receptionList.map(async (reception) => {
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
            return reception;
          })
        );
      } catch (error) {
        console.error('Ошибка при загрузке приемов:', error);
        this.receptions = [];
      }
    }
  },

  async mounted() {
    await this.fetchTests();
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