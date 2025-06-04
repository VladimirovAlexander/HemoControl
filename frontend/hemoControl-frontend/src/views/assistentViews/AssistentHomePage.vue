<template>
  <div class="space-y-6">
    <!-- Заголовок -->
    <h1 class="text-3xl font-bold text-blue-900">Добро пожаловать, лаборант!</h1>

    <!-- Карточки сводки -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Всего анализов</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.totalTests }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Назначено сегодня</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.todaysTests }}</p>
      </div>
      <div class="bg-white p-6 rounded-2xl shadow">
        <h2 class="text-sm text-gray-500">Ожидают выполнения</h2>
        <p class="text-2xl font-semibold text-blue-800">{{ stats.pendingTests }}</p>
      </div>
    </div>

    <!-- Список анализов -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold text-blue-800">Анализы для выполнения</h2>
        <div class="flex space-x-2">
          <select v-model="filterStatus" class="border rounded p-2 text-sm">
            <option value="all">Все статусы</option>
            <option value="pending">Ожидают выполнения</option>
            <option value="in_progress">В процессе</option>
            <option value="completed">Завершены</option>
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
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Тип анализа</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Назначен</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Статус</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="test in filteredTests" :key="test.id">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div>
                    <div class="font-medium text-blue-900">
                      {{ test.patient.surname }} {{ test.patient.name }} {{ test.patient.patronymic }}
                    </div>
                    <div class="text-sm text-gray-500">Полис: {{ test.patient.policy }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ getTestTypeName(test.testType) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(test.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="statusClasses(test.status)" class="px-2 py-1 text-xs rounded-full">
                  {{ getStatusName(test.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <button
                  @click="openTestModal(test)"
                  class="text-blue-600 hover:text-blue-900 mr-3"
                >
                  Заполнить
                </button>
              </td>
            </tr>
            <tr v-if="filteredTests.length === 0">
              <td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">
                Нет анализов для отображения
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Модальное окно заполнения анализов -->
    <div
      v-if="showTestModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 p-4 overflow-auto"
      @click.self="closeTestModal"
    >
      <div class="bg-white rounded-lg w-full max-w-4xl max-h-screen flex flex-col shadow-xl overflow-auto">
        <!-- Шапка модального окна -->
        <div class="flex justify-between items-center border-b p-4 bg-white">
          <h2 class="text-2xl font-semibold text-blue-900">
            Заполнение анализа: {{ getTestTypeName(selectedTest.testType) }}
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
            <!-- Информация о пациенте -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Пациент</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p><span class="font-medium">ФИО:</span> {{ selectedTest.patient.surname }} {{ selectedTest.patient.name }} {{ selectedTest.patient.patronymic }}</p>
                  <p><span class="font-medium">Дата рождения:</span> {{ formatDate(selectedTest.patient.dateOfBirth) }}</p>
                </div>
                <div>
                  <p><span class="font-medium">Полис:</span> {{ selectedTest.patient.policy }}</p>
                  <p><span class="font-medium">Назначен:</span> {{ formatDateTime(selectedTest.createdAt) }}</p>
                </div>
              </div>
            </div>

            <!-- Форма заполнения анализов -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Результаты анализа</h3>
              
              <!-- Общий анализ крови -->
              <div v-if="selectedTest.testType === 'CBC'" class="border p-4 rounded shadow">
                <h4 class="text-blue-700 font-semibold mb-2">Общий анализ крови</h4>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium mb-1">Гемоглобин (г/л)</label>
                    <input v-model.number="testResults.hemoglobin" class="input" type="number" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Гематокрит (%)</label>
                    <input v-model.number="testResults.hematocrit" class="input" type="number" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Лейкоциты (×10⁹/л)</label>
                    <input v-model.number="testResults.whiteBloodCells" class="input" type="number" step="0.01" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Эритроциты (×10¹²/л)</label>
                    <input v-model.number="testResults.redBloodCells" class="input" type="number" step="0.01" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Тромбоциты (×10⁹/л)</label>
                    <input v-model.number="testResults.platelets" class="input" type="number" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">MCH (пг)</label>
                    <input v-model.number="testResults.mch" class="input" type="number" step="0.1" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">MCV (фл)</label>
                    <input v-model.number="testResults.mcv" class="input" type="number" step="0.1" />
                  </div>
                </div>
              </div>

              <!-- Коагулограмма -->
              <div v-if="selectedTest.testType === 'Coagulogram'" class="border p-4 rounded shadow">
                <h4 class="text-blue-700 font-semibold mb-2">Коагулограмма</h4>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium mb-1">PT (сек)</label>
                    <input v-model.number="testResults.pt" class="input" type="number" step="0.1" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">INR</label>
                    <input v-model.number="testResults.inr" class="input" type="number" step="0.01" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">APTT (сек)</label>
                    <input v-model.number="testResults.aptt" class="input" type="number" step="0.1" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Фибриноген (г/л)</label>
                    <input v-model.number="testResults.fibrinogen" class="input" type="number" step="0.01" />
                  </div>
                </div>
              </div>

              <!-- Факторы свертывания -->
              <div v-if="selectedTest.testType === 'FactorAndVWF'" class="border p-4 rounded shadow">
                <h4 class="text-blue-700 font-semibold mb-2">Факторы свертывания и VWF</h4>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium mb-1">Фактор VIII (%)</label>
                    <input v-model.number="testResults.factorVIII" class="input" type="number" step="0.1" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Фактор IX (%)</label>
                    <input v-model.number="testResults.factorIX" class="input" type="number" step="0.1" />
                  </div>
                  <div>
                    <label class="block text-sm font-medium mb-1">Активность VWF (%)</label>
                    <input v-model.number="testResults.vwfActivity" class="input" type="number" step="0.1" />
                  </div>
                </div>
              </div>

              <!-- Комментарий -->
              <div class="mt-4">
                <label class="block text-sm font-medium mb-1">Комментарий лаборанта</label>
                <textarea
                  v-model="testResults.result"
                  class="w-full border rounded p-2 min-h-[80px]"
                  placeholder="Введите комментарий к анализу..."
                ></textarea>
              </div>

              <!-- Статус -->
              <div class="mt-4">
                <label class="block text-sm font-medium mb-1">Статус анализа</label>
                <select v-model="testResults.status" class="border rounded p-2 w-full">
                  <option value="pending">Ожидает выполнения</option>
                  <option value="in_progress">В процессе</option>
                  <option value="completed">Завершен</option>
                </select>
              </div>
            </div>
          </div>
        </div>

        <!-- Кнопки сохранения -->
        <div class="border-t p-4 bg-white flex justify-between">
          <button
            @click="closeTestModal"
            class="px-6 py-2 border rounded-lg text-gray-700 hover:bg-gray-100 transition"
          >
            Отмена
          </button>
          <div>
            <button
              @click="saveTestResults(false)"
              class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition mr-2"
            >
              Сохранить
            </button>
            <button
              @click="saveTestResults(true)"
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
  name: 'LabAssistantPage',
  data() {
    return {
      stats: {
        totalTests: 0,
        todaysTests: 0,
        pendingTests: 0,
      },
      tests: [],
      searchQuery: '',
      filterStatus: 'all',
      showTestModal: false,
      selectedTest: {
        id: null,
        testType: '',
        patient: {
          surname: '',
          name: '',
          patronymic: '',
          dateOfBirth: '',
          policy: ''
        },
        createdAt: ''
      },
      testResults: {
        status: 'pending',
        result: '',
        // CBC
        hemoglobin: null,
        hematocrit: null,
        whiteBloodCells: null,
        redBloodCells: null,
        platelets: null,
        mch: null,
        mcv: null,
        // Coagulogram
        pt: null,
        inr: null,
        aptt: null,
        fibrinogen: null,
        // FactorAndVWF
        factorVIII: null,
        factorIX: null,
        vwfActivity: null
      }
    };
  },
  computed: {
    filteredTests() {
      let filtered = this.tests;

      // Фильтрация по статусу
      if (this.filterStatus !== 'all') {
        filtered = filtered.filter(test => test.status === this.filterStatus);
      }

      // Фильтрация по поисковому запросу
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(test => 
          test.patient.surname.toLowerCase().includes(query) ||
          test.patient.name.toLowerCase().includes(query) ||
          test.patient.patronymic.toLowerCase().includes(query) ||
          test.patient.policy.toLowerCase().includes(query)
        );
      }

      return filtered;
    }
  },
  methods: {
    getTestTypeName(testType) {
      const types = {
        'CBC': 'Общий анализ крови',
        'Coagulogram': 'Коагулограмма',
        'FactorAndVWF': 'Факторы свертывания и VWF'
      };
      return types[testType] || testType;
    },
    getStatusName(status) {
      const statuses = {
        'pending': 'Ожидает выполнения',
        'in_progress': 'В процессе',
        'completed': 'Завершен'
      };
      return statuses[status] || status;
    },
    statusClasses(status) {
      return {
        'bg-yellow-100 text-yellow-800': status === 'pending',
        'bg-blue-100 text-blue-800': status === 'in_progress',
        'bg-green-100 text-green-800': status === 'completed'
      };
    },
    mapStatusToBackend(frontendStatus) {
      // Преобразуем в точное соответствие с enum на бэкенде
      const statusMap = {
        'pending': 'Pending',
        'in_progress': 'InProgress',
        'completed': 'Completed',
        'canceled': 'Canceled'
      };
      return statusMap[frontendStatus] || 'Pending';
    },
    
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString();
    },
    formatDateTime(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleString();
    },
    getTestResults() {
      const type = this.selectedTest.testType;
      if (type === 'CBC') {
        return {
          hemoglobin: this.testResults.hemoglobin,
          hematocrit: this.testResults.hematocrit,
          whiteBloodCells: this.testResults.whiteBloodCells,
          redBloodCells: this.testResults.redBloodCells,
          platelets: this.testResults.platelets,
          mch: this.testResults.mch,
          mcv: this.testResults.mcv
        };
      } else if (type === 'Coagulogram') {
        return {
          pt: this.testResults.pt,
          inr: this.testResults.inr,
          aptt: this.testResults.aptt,
          fibrinogen: this.testResults.fibrinogen
        };
      } else if (type === 'FactorAndVWF') {
        return {
          factorVIII: this.testResults.factorVIII,
          factorIX: this.testResults.factorIX,
          vwfActivity: this.testResults.vwfActivity
        };
      }
      return {};
    },
    async fetchTests() {
      try {
        const response = await axios.get('https://localhost:7098/api/tests', {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        this.tests = response.data;
        this.updateStats();
      } catch (error) {
        console.error('Ошибка при загрузке анализов:', error);
      }
    },
    updateStats() {
      this.stats.totalTests = this.tests.length;
      
      const today = new Date().toISOString().split('T')[0];
      this.stats.todaysTests = this.tests.filter(test => 
        new Date(test.createdAt).toISOString().split('T')[0] === today
      ).length;
      
      this.stats.pendingTests = this.tests.filter(test => 
        test.status === 'pending'
      ).length;
    },
    openTestModal(test) {
      this.selectedTest = { ...test };
      
      // Заполняем результаты в зависимости от типа теста
      this.testResults = {
        status: test.status || 'pending',
        result: test.result || '',
        // CBC
        hemoglobin: test.cbcDetails?.hemoglobin || null,
        hematocrit: test.cbcDetails?.hematocrit || null,
        whiteBloodCells: test.cbcDetails?.whiteBloodCells || null,
        redBloodCells: test.cbcDetails?.redBloodCells || null,
        platelets: test.cbcDetails?.platelets || null,
        mch: test.cbcDetails?.mch || null,
        mcv: test.cbcDetails?.mcv || null,
        // Coagulogram
        pt: test.coagulogramDetails?.pt || null,
        inr: test.coagulogramDetails?.inr || null,
        aptt: test.coagulogramDetails?.aptt || null,
        fibrinogen: test.coagulogramDetails?.fibrinogen || null,
        // FactorAndVWF
        factorVIII: test.factorAndVwfDetails?.factorVIII || null,
        factorIX: test.factorAndVwfDetails?.factorIX || null,
        vwfActivity: test.factorAndVwfDetails?.vwfActivity || null
      };
      
      this.showTestModal = true;
    },
    closeTestModal() {
      this.showTestModal = false;
      this.selectedTest = {
        id: null,
        testType: '',
        patient: {
          surname: '',
          name: '',
          patronymic: '',
          dateOfBirth: '',
          policy: ''
        },
        createdAt: ''
      };
      this.resetTestResults();
    },
    resetTestResults() {
      this.testResults = {
        status: 'pending',
        result: '',
        // CBC
        hemoglobin: null,
        hematocrit: null,
        whiteBloodCells: null,
        redBloodCells: null,
        platelets: null,
        mch: null,
        mcv: null,
        // Coagulogram
        pt: null,
        inr: null,
        aptt: null,
        fibrinogen: null,
        // FactorAndVWF
        factorVIII: null,
        factorIX: null,
        vwfActivity: null
      };
    },
    async saveTestResults(closeAfterSave) {
      try {
        const payload = {
          Dto: {
            testId: this.selectedTest.id,
            status: this.mapStatusToBackend(this.testResults.status), // Преобразуем статус
            result: this.testResults.result,
            testType: this.selectedTest.testType,
            results: this.getTestResults()
          }
        };
        console.log('Sending payload:', JSON.stringify(payload, null, 2));
        const response = await axios.put(
          'https://localhost:7098/api/tests/update', 
          payload,
          {
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          }
        );

        await this.fetchTests();
        alert('Результаты анализа успешно сохранены');
        if (closeAfterSave) this.closeTestModal();
      } catch (error) {
        console.error('Ошибка при сохранении:', error.response?.data || error.message);
        alert('Ошибка при сохранении: ' + (error.response?.data?.title || error.message));
      }
    },
  },
  async mounted() {
    await this.fetchTests();
  }
};
</script>

<style>
/* Стили для скроллбара */
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