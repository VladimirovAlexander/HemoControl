<template>
  <div class="p-6 max-w-7xl mx-auto">
    <div class="bg-white shadow-xl rounded-2xl p-6">
      <h1 class="text-3xl font-bold text-blue-800 mb-6 text-center">Мои приёмы</h1>

      <div v-if="loading" class="text-center text-gray-500">Загрузка...</div>
      <div v-else-if="receptions.length === 0" class="text-center text-gray-500">Нет записей.</div>

      <div v-else class="flex flex-col gap-6">
        <div
          v-for="reception in receptions"
          :key="reception.id"
          class="bg-white border rounded-2xl p-6 shadow hover:shadow-lg transition w-full"
        >
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-xl font-semibold text-gray-800 mb-2">Приём №{{ reception.id.slice(0, 8) }}</h2>
              <p class="mb-1"><span class="font-semibold">Пациент:</span> {{ formatPatientFull(reception.patient) }}</p>
              <p class="mb-1"><span class="font-semibold">Полис:</span> {{ reception.patient?.policy || 'Не указан' }}</p>
              <p class="mb-1"><span class="font-semibold">Адрес:</span> {{ formatPatientAddress(reception.patient) }}</p>

              <p class="mb-1"><span class="font-semibold">Врач:</span> {{ formatDoctor(reception.doctor) }}</p>
              <p class="mb-1"><span class="font-semibold">Специализация:</span> {{ reception.doctor?.specialty || 'Не указано' }}</p>

              <p class="mb-1">
                <span class="font-semibold">Дата:</span> {{ formatDate(reception.slotInfo?.startDateTime) }}
              </p>
              <p class="mb-1">
                <span class="font-semibold">Время:</span> {{ formatTime(reception.slotInfo?.startDateTime) }} — {{ formatTime(reception.slotInfo?.endDateTime) }}
              </p>

              <p class="mb-3"><span class="font-semibold">Статус:</span>
                <span :class="statusColor(reception.status)">
                  {{ reception.status }}
                </span>
              </p>
            </div>

            <button
              @click="toggleDetails(reception.id)"
              class="text-blue-600 hover:underline"
            >
              {{ expandedIds.includes(reception.id) ? 'Скрыть' : 'Подробнее' }}
            </button>
          </div>

          <div v-if="expandedIds.includes(reception.id)" class="mt-4 border-t pt-4 text-gray-700 space-y-2">
            <p><span class="font-semibold">Описание:</span> {{ reception.description || 'Нет описания' }}</p>
            <p><span class="font-semibold">Жалобы:</span> {{ reception.complaints || 'Не указано' }}</p>
            <p><span class="font-semibold">Рекомендации:</span> {{ reception.recommendations || 'Нет рекомендаций' }}</p>

            <button
              v-if="reception.status === 'Запланировано'"
              @click="cancelReception(reception.id)"
              class="w-full mt-4 bg-red-600 hover:bg-red-700 text-white py-2 rounded-lg"
            >
              Отменить приём
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { format } from 'date-fns'
import { ru } from 'date-fns/locale'

const receptions = ref([])
const loading = ref(true)
const expandedIds = ref([])

const toggleDetails = (id) => {
  if (expandedIds.value.includes(id)) {
    expandedIds.value = expandedIds.value.filter(i => i !== id)
  } else {
    expandedIds.value.push(id)
  }
}


const fetchReceptions = async () => {
  loading.value = true
  const patientId = localStorage.getItem('patientId')

  try {
    const response = await axios.get(`https://localhost:7098/api/reception/user-receptions/${patientId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    })

    const receptionList = response.data || []

    const enrichedReceptions = await Promise.all(
      receptionList.map(async (reception) => {
        // Получаем доктора
        try {
          const doctorRes = await axios.get(`https://localhost:7098/api/doctor/${reception.doctorId}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          })
          reception.doctor = doctorRes.data
        } catch {
          reception.doctor = null
        }

        // Получаем слот
        try {
          const slotRes = await axios.get(`https://localhost:7098/api/schedule/get-slot/${reception.slotId}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          })
          reception.slotInfo = slotRes.data
        } catch {
          reception.slotInfo = null
        }

        // Получаем пациента
        try {
          const patientRes = await axios.get(`https://localhost:7098/api/patient/${reception.patientId}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
          })
          reception.patient = patientRes.data
        } catch {
          reception.patient = null
        }

        return reception
      })
    )

    receptions.value = enrichedReceptions
  } catch (error) {
    console.error('Ошибка при загрузке приёмов:', error)
  } finally {
    loading.value = false
  }
}

const cancelReception = async (id) => {
  if (!confirm('Вы уверены, что хотите отменить приём?')) return
  try {
    await axios.delete(`https://localhost:7098/api/reception/${id}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    })
    receptions.value = receptions.value.map(r =>
      r.id === id ? { ...r, status: 'Отменён' } : r
    )
  } catch (error) {
    console.error('Ошибка при отмене приёма:', error)
  }
}

const formatDoctor = (doctor) => {
  if (!doctor) return 'Неизвестно'
  return `${doctor.lastName} ${doctor.firstName} (${doctor.specialty})`
}

const formatPatientFull = (patient) => {
  if (!patient) return 'Неизвестно'
  return `${patient.surname} ${patient.name} ${patient.patronymic}, ${formatDate(patient.dateOfBirth)} г.р.`
}

const formatPatientAddress = (patient) => {
  if (!patient) return ''
  return `${patient.country}, ${patient.region}, ${patient.city}, ул. ${patient.street}, д. ${patient.houseNumber}, кв. ${patient.appartmentNumber}`
}

const formatDate = (dateTimeString) => {
  if (!dateTimeString) return 'Не указано'
  return format(new Date(dateTimeString), 'd MMMM yyyy', { locale: ru })
}

const formatTime = (dateTimeString) => {
  if (!dateTimeString) return ''
  return format(new Date(dateTimeString), 'HH:mm')
}

const statusColor = (status) => {
  switch (status) {
    case 'Запланировано':
      return 'text-green-600 font-semibold'
    case 'Отменён':
      return 'text-red-600 font-semibold'
    default:
      return 'text-gray-600'
  }
}

onMounted(fetchReceptions)
</script>


<style scoped>
</style>
