<template>
  <div class="p-6 max-w-7xl mx-auto">
    <h2 class="text-3xl font-bold text-blue-800 mb-6 text-center">Все приёмы</h2>

    <!-- Список всех приёмов -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <ul class="divide-y">
        <li
          class="py-4 flex flex-col space-y-2"
        >
          <div class="flex justify-between items-center">
            <div>
              <p class="font-medium text-blue-900">
                {{ appt.patient.surname }} {{ appt.patient.name }} {{ appt.patient.patronymic }}
              </p>
              <p class="text-sm text-gray-600">
                {{ formatDate(appt.slot.startDateTime) }} — {{ formatTime(appt.slot.startDateTime) }}
              </p>
            </div>
            <button
              class="text-blue-600 hover:underline text-sm"
              @click="openCard(appt)"
            >
              Открыть карту
            </button>
          </div>

          <div class="text-sm text-gray-700 space-y-1">
            <p><span class="font-semibold">Статус:</span> {{ appt.status }}</p>
            <p v-if="appt.notes?.complaints"><span class="font-semibold">Жалобы:</span> {{ appt.notes.complaints }}</p>
            <p v-if="appt.notes?.treatment"><span class="font-semibold">Лечение:</span> {{ appt.notes.treatment }}</p>
            <p v-if="appt.notes?.recommendations"><span class="font-semibold">Рекомендации:</span> {{ appt.notes.recommendations }}</p>
          </div>
        </li>
      </ul>
    </div>

    <!-- Модальное окно с исправленной прокруткой -->
    <div
      v-if="showModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50 p-4 overflow-auto"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg w-full max-w-4xl max-h-screen flex flex-col shadow-xl overflow-auto">
        <!-- Шапка модального окна -->
        <div class="flex justify-between items-center border-b p-4 bg-white">
          <h2 class="text-2xl font-semibold text-blue-900">Карточка приёма</h2>
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
            <!-- Секция пациента -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Пациент</h3>
              <div class="space-y-2">
                <p>
                  <span class="font-medium">ФИО:</span>
                  {{ selectedAppointment.patient.surname }}
                  {{ selectedAppointment.patient.name }}
                  {{ selectedAppointment.patient.patronymic }}
                </p>
                <p><span class="font-medium">Дата рождения:</span> {{ formatDate(selectedAppointment.patient.dateOfBirth) }}</p>
                <p>
                  <span class="font-medium">Адрес:</span>
                  {{ selectedAppointment.patient.country }},
                  {{ selectedAppointment.patient.region }},
                  {{ selectedAppointment.patient.city }},
                  ул. {{ selectedAppointment.patient.street }},
                  д. {{ selectedAppointment.patient.houseNumber }},
                  кв. {{ selectedAppointment.patient.appartmentNumber }}
                </p>
                <p><span class="font-medium">Полис:</span> {{ selectedAppointment.patient.policy }}</p>
              </div>
            </div>

            <!-- Секция приема -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Приём</h3>
              <div class="space-y-2">
                <p><span class="font-medium">Статус:</span> {{ selectedAppointment.status }}</p>
                <p>
                  <span class="font-medium">Дата и время:</span>
                  {{ formatDate(selectedAppointment.slot.startDateTime) }}
                  {{ formatTime(selectedAppointment.slot.startDateTime) }} -
                  {{ formatTime(selectedAppointment.slot.endDateTime) }}
                </p>
                <p>
                  <span class="font-medium">Создан:</span>
                  {{ formatDate(selectedAppointment.createdAt) }}
                  {{ formatTime(selectedAppointment.createdAt) }}
                </p>
              </div>
            </div>

            <!-- Секция заметок -->
            <div>
              <h3 class="text-lg font-semibold mb-2 text-blue-800">Заметки</h3>
              
              <div class="space-y-4">
                
                <!-- Анамнез -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Анамнез</h4>
                  <textarea
                    v-model="notes.anamnesis"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder=""
                    ref="anamnesisTextarea"
                    @input="autoResize($refs.anamnesisTextarea)"
                  ></textarea>
                </div>

                <!-- Жалобы -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Жалобы</h4>
                  <textarea
                    v-model="notes.complaints"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder="Опишите жалобы пациента"
                    ref="complaintsTextarea"
                    @input="autoResize($refs.complaintsTextarea)"
                  ></textarea>
                </div>

                <!-- Объективный статус -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Объективный статус</h4>
                  <textarea
                    v-model="notes.generalConditions"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder="Опишите состояние пациента"
                    ref="generalConditionsTextarea"
                    @input="autoResize($refs.generalConditionsTextarea)"
                  ></textarea>
                </div>

                <!-- Физика -->
                <div>
                  <h4 class="font-semibold mb-2 text-blue-700">Физика</h4>
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label class="block text-sm font-medium mb-1">Телосложение</label>
                      <input
                        type="text"
                        v-model.number="notes.phisique"
                        class="w-full border rounded p-2"
                      />
                    </div>
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
                  </div>
                </div>

                <!-- Другие поля -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Кожа</h4>
                  <textarea
                    v-model="notes.skin"
                    class="w-full border rounded p-2 min-h-[60px] resize-none overflow-hidden"
                    placeholder="Опишите состояние кожи"
                    ref="skinTextarea"
                    @input="autoResize($refs.skinTextarea)"
                  ></textarea>
                </div>

                <!-- Осмотр -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Осмотр</h4>
                  <textarea
                    v-model="notes.examination"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder="Результаты осмотра"
                    ref="examinationTextarea"
                    @input="autoResize($refs.examinationTextarea)"
                  ></textarea>
                </div>

                <!-- Лечение -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Лечение</h4>
                  <textarea
                    v-model="notes.treatment"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder="Назначенное лечение"
                    ref="treatmentTextarea"
                    @input="autoResize($refs.treatmentTextarea)"
                  ></textarea>
                </div>

                <!-- Рекомендации -->
                <div>
                  <h4 class="font-semibold mb-1 text-blue-700">Рекомендации</h4>
                  <textarea
                    v-model="notes.recommendations"
                    class="w-full border rounded p-2 min-h-[100px] resize-none overflow-hidden"
                    placeholder="Рекомендации пациенту"
                    ref="recommendationsTextarea"
                    @input="autoResize($refs.recommendationsTextarea)"
                  ></textarea>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Кнопка сохранения -->
        <div class="border-t p-4 bg-white">
          <button
            @click="saveNotes"
            class="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition"
          >
            Сохранить изменения
          </button>
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
      showModal: false,
      selectedAppointment: null,
      notes: {}
    };
  },
  methods: {
    openCard(appt) {
      this.selectedAppointment = appt;
      this.notes = { ...appt.notes };
      this.showModal = true;
    },
    closeModal() {
      this.showModal = false;
      this.selectedAppointment = null;
      this.notes = {};
    },
    formatDate(datetime) {
      if (!datetime) return '';
      const date = new Date(datetime);
      return date.toLocaleDateString();
    },
    formatTime(datetime) {
      if (!datetime) return '';
      const date = new Date(datetime);
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    },
    async getDoctorId() {
      try {
        const response = await axios.get(`https://localhost:7098/api/doctor/getDoctorByUserId/${localStorage.getItem('userId')}`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        this.doctorId = response.data.id;
      } catch (error) {
        console.error('Ошибка при получении ID врача:', error);
      }
    },
    async getDoctorReceptions() {
      if (!this.doctorId) return;
      try {
        const response = await axios.get(`https://localhost:7098/api/reception/doctor-receptions/${this.doctorId}`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        const data = response.data;
        this.upcomingAppointments = data;
        const today = new Date().toISOString().split('T')[0];
        this.stats.todaysAppointments = data.filter(appt =>
          appt.slot?.startDateTime?.startsWith(today)
        ).length;
      } catch (error) {
        console.error('Ошибка при загрузке приёмов:', error);
      }
    },
    async saveNotes() {
      if (!this.selectedAppointment || !this.notes?.id) return;

      // Сформировать DTO, исключая id
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
        turnout: this.notes.turnout
      };

      try {
        await axios.put(`https://localhost:7098/api/notes/updateNote/${this.notes.id}`, updateNotesDto, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        Object.assign(this.selectedAppointment.notes, this.notes);
        alert('Заметки успешно сохранены');
        this.closeModal();
      } catch (error) {
        console.error('Ошибка при сохранении заметок:', error);
        alert('Не удалось сохранить заметки');
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