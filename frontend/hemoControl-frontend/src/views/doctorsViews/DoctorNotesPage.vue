<template>
  <div class="p-6 max-w-7xl mx-auto">
    <h2 class="text-3xl font-bold text-blue-800 mb-6 text-center">Приемы на сегодня</h2>

    <!-- Список приемов -->
    <div v-if="receptions.length" class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-10">
      <div
        v-for="reception in receptions"
        :key="reception.id"
        @click="selectReception(reception)"
        class="cursor-pointer bg-white shadow-lg rounded-2xl p-4 hover:bg-blue-50 transition border border-blue-100"
        :class="{ 'ring ring-blue-500': selectedReception?.id === reception.id }"
      >
        <p class="font-semibold text-lg text-gray-800">Пациент: {{ reception.patientFullName }}</p>
        <p class="text-gray-600">Время: {{ reception.time.slice(0, 5) }}</p>
      </div>
    </div>
    <p v-else class="text-center text-gray-600">Нет записей на сегодня</p>

    <!-- Форма осмотра -->
    <div v-if="selectedReception" class="bg-white shadow-2xl rounded-2xl p-6">
      <h3 class="text-2xl font-semibold text-blue-700 mb-4 text-center">Заполните результаты осмотра</h3>

      <form @submit.prevent="submitNotes" class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div v-for="(field, key) in formFields" :key="key">
          <label class="block font-medium text-gray-700 mb-1">{{ field.label }}</label>
          <input
            v-model="form[key]"
            :type="field.type || 'text'"
            class="w-full border rounded-lg px-4 py-2"
            :placeholder="field.placeholder"
          />
        </div>

        <div class="col-span-full text-center mt-4">
          <button
            type="submit"
            class="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg"
          >
            Сохранить осмотр
          </button>
        </div>
      </form>
    </div>

    <!-- Успешное сохранение -->
    <div
      v-if="successMessage"
      class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50"
    >
      <div class="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full">
        <h3 class="text-xl font-bold text-center text-green-600 mb-4">Сохранено</h3>
        <p class="text-center mb-4">{{ successMessage }}</p>
        <div class="text-center">
          <button @click="successMessage = null" class="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg">
            Ок
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import { format } from 'date-fns'

export default {
  data() {
    return {
      receptions: [],
      selectedReception: null,
      form: {
        anamnesis: '',
        complaints: '',
        generalConditions: '',
        physique: '',
        weight: '',
        height: '',
        bloodPressureSystolic: '',
        bloodPressureDiastolic: '',
        temperature: '',
        state: '',
        skin: '',
        examination: '',
        treatment: '',
        recommendations: '',
        turnout: '',
      },
      successMessage: null,
    }
  },
  computed: {
    formFields() {
      return {
        anamnesis: { label: 'Анамнез', placeholder: 'Опишите анамнез' },
        complaints: { label: 'Жалобы', placeholder: 'Жалобы пациента' },
        generalConditions: { label: 'Общее состояние', placeholder: '' },
        physique: { label: 'Телосложение', placeholder: '' },
        weight: { label: 'Вес (кг)', type: 'number' },
        height: { label: 'Рост (см)', type: 'number' },
        bloodPressureSystolic: { label: 'Систолическое давление', type: 'number' },
        bloodPressureDiastolic: { label: 'Диастолическое давление', type: 'number' },
        temperature: { label: 'Температура тела', type: 'number' },
        state: { label: 'Состояние', placeholder: '' },
        skin: { label: 'Кожные покровы', placeholder: '' },
        examination: { label: 'Осмотр', placeholder: '' },
        treatment: { label: 'Назначения/Лечение', placeholder: '' },
        recommendations: { label: 'Рекомендации', placeholder: '' },
        turnout: { label: 'Явка', placeholder: '' },
      }
    }
  },
  mounted() {
    this.fetchTodayReceptions()
  },
  methods: {
    async fetchTodayReceptions() {
      const doctorId = localStorage.getItem('doctorId')
      const today = format(new Date(), 'yyyy-MM-dd')

      try {
        const res = await axios.get(`https://localhost:7098/api/reception/today`, {
          params: { doctorId, date: today },
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        })
        this.receptions = res.data
      } catch (err) {
        console.error('Ошибка загрузки приемов:', err)
      }
    },
    selectReception(reception) {
      this.selectedReception = reception
      this.resetForm()
    },
    resetForm() {
      Object.keys(this.form).forEach(key => this.form[key] = '')
    },
    async submitNotes() {
      const dto = {
        receptionId: this.selectedReception.id,
        ...this.form,
      }

      try {
        await axios.post(`https://localhost:7098/api/notes`, dto, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        })

        this.successMessage = 'Результаты осмотра сохранены'
        this.selectedReception = null
        this.resetForm()
        this.fetchTodayReceptions()
      } catch (err) {
        console.error('Ошибка при сохранении осмотра:', err)
        alert('Не удалось сохранить осмотр')
      }
    }
  }
}
</script>

<style scoped>
input, textarea {
  transition: border 0.2s;
}
input:focus, textarea:focus {
  border-color: #2563eb;
  outline: none;
}
</style>