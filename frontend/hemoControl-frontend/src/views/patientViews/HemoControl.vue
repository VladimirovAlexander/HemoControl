<template>
  <div class="space-y-8">
    <!-- Приветствие и краткая статистика -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 class="text-3xl font-bold text-blue-900">Добро пожаловать, {{ userName }}!</h1>
          <p class="text-gray-600 mt-2">Мы заботимся о вашем здоровье</p>
        </div>
        <div class="grid grid-cols-2 rg:grid-cols-4 gap-4">
          <div class="bg-blue-50 p-3 rounded-lg text-center">
            <p class="text-sm text-blue-800">Дата и время</p>
            <p class="font-semibold">{{ currentDate }}</p>
          </div>
          <div class="bg-green-50 p-5 rounded-lg text-center">
            <p class="text-sm text-green-800">Записанных приемов</p>
            <p class="font-semibold">{{ upcomingAppointments }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Ближайшие приемы -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold text-blue-800">Ближайшие приемы</h2>
        <router-link 
          to="/hemocontrol/appointments" 
          class="text-blue-600 hover:text-blue-800 text-sm"
        >
          Все приемы →
        </router-link>
      </div>
      
      <div v-if="nextAppointments.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div 
          v-for="appointment in nextAppointments.slice(0, 3)" 
          :key="appointment.id"
          class="border rounded-lg p-4 hover:shadow-md transition"
        >
          <div class="flex items-center gap-3 mb-2">
            <div class="bg-blue-100 p-2 rounded-full">
              <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
              </svg>
            </div>
            <div>
              <p class="text-sm text-gray-500">{{ formatTime(appointment.slot.startTime) }}</p>
            </div>
          </div>
          <h3 class="font-semibold text-lg">{{ appointment.doctor.specialty }}</h3>
          <p class="text-gray-700">Доктор: {{ appointment.doctor.surname }} {{ appointment.doctor.name }} {{ appointment.doctor.patronymic }}</p>
          <div class="mt-3 flex justify-between items-center">
            <span :class="statusBadgeClass(appointment.status)" class="px-2 py-1 text-xs rounded-full">
              {{ getStatusText(appointment.status) }}
            </span>
          </div>
        </div>
      </div>
      <div v-else class="text-center py-8 bg-gray-50 rounded-lg">
        <p class="text-gray-500">У вас нет запланированных приемов</p>
      </div>
    </div>

    <!-- Полезные статьи и рекомендации -->
    <div class="bg-white p-6 rounded-2xl shadow">
      <h2 class="text-xl font-semibold text-blue-800 mb-4">Полезные материалы</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div 
          v-for="article in healthArticles" 
          :key="article.id"
          class="border rounded-lg overflow-hidden hover:shadow-md transition"
        >
          <img :src="article.image" :alt="article.title" class="w-full h-40 object-cover">
          <div class="p-4">
            <div class="flex justify-between items-start mb-2">
              <span class="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded">{{ article.category }}</span>
              <span class="text-gray-500 text-sm">{{ article.date }}</span>
            </div>
            <h3 class="font-semibold text-lg mb-2">{{ article.title }}</h3>
            <p class="text-gray-600 text-sm mb-3">{{ article.excerpt }}</p>
            <button 
              @click="openArticleModal(article)"
              class="text-blue-600 hover:text-blue-800 text-sm font-medium"
            >
              Читать далее →
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно для статей -->
    <div v-if="selectedArticle" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] flex flex-col">
        <!-- Шапка модального окна -->
        <div class="p-6 border-b sticky top-0 bg-white z-10">
          <div class="flex justify-between items-start">
            <div>
              <span class="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded">{{ selectedArticle.category }}</span>
              <span class="text-gray-500 text-sm ml-2">{{ selectedArticle.date }}</span>
            </div>
            <button @click="selectedArticle = null" class="text-gray-500 hover:text-gray-700">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
          <h3 class="text-2xl font-bold mt-2">{{ selectedArticle.title }}</h3>
        </div>
        
        <!-- Прокручиваемый контент -->
        <div class="overflow-y-auto flex-1 p-6">
          <img :src="selectedArticle.image" :alt="selectedArticle.title" class="w-full h-48 object-cover mb-4 rounded">
          <div class="prose max-w-none" v-html="selectedArticle.content"></div>
        </div>
        
        <!-- Нижняя часть с кнопкой -->
        <div class="p-4 border-t sticky bottom-0 bg-white">
          <button 
            @click="selectedArticle = null"
            class="w-full px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Закрыть
          </button>
        </div>
      </div>
    </div>

    <!-- Быстрый доступ -->
    <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
      <router-link 
        to="/hemocontrol/appointments/book" 
        class="bg-white p-4 rounded-lg shadow text-center hover:shadow-md transition"
      >
        <div class="bg-blue-100 p-3 rounded-full inline-block mb-2">
          <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
          </svg>
        </div>
        <p class="font-medium">Записаться на прием</p>
      </router-link>
      
      <router-link 
        to="/hemocontrol/lab-results" 
        class="bg-white p-4 rounded-lg shadow text-center hover:shadow-md transition"
      >
        <div class="bg-yellow-100 p-3 rounded-full inline-block mb-2">
          <svg class="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path>
          </svg>
        </div>
        <p class="font-medium">Результаты анализов</p>
      </router-link>
      
      <router-link 
        to="/hemocontrol/appointments" 
        class="bg-white p-4 rounded-lg shadow text-center hover:shadow-md transition"
      >
        <div class="bg-purple-100 p-3 rounded-full inline-block mb-2">
          <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
          </svg>
        </div>
        <p class="font-medium">Мои приемы</p>
      </router-link>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'PatientDashboard',
  data() {
    return {
      currentDate: new Date().toLocaleDateString('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }),
      userName: "",
      upcomingAppointments: 0,
      nextAppointments: [],
      healthArticles: [
        {
          id: 1,
          title: 'Как подготовиться к сдаче анализов крови',
          excerpt: 'Правильная подготовка обеспечит точные результаты ваших анализов...',
          category: 'Анализы',
          date: '10.06.2023',
          image: 'https://images.unsplash.com/photo-1579684385127-1ef15d508118?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&q=80',
          content: `
            <h4>Подготовка к сдаче анализов крови</h4>
            <p>Для получения достоверных результатов анализов крови важно правильно подготовиться:</p>
            <ul class="list-disc pl-5 space-y-2">
              <li><strong>Голодание:</strong> Большинство анализов крови сдаются натощак (8-12 часов без пищи). Можно пить воду.</li>
              <li><strong>Время суток:</strong> Лучше сдавать анализы утром, так как показатели крови могут колебаться в течение дня.</li>
              <li><strong>Алкоголь:</strong> Исключите алкоголь за 24-48 часов до анализа.</li>
              <li><strong>Физические нагрузки:</strong> Избегайте интенсивных тренировок за день до анализа.</li>
              <li><strong>Лекарства:</strong> Сообщите врачу о принимаемых препаратах, некоторые могут влиять на результаты.</li>
              <li><strong>Курение:</strong> Не курите минимум за 1 час до сдачи крови.</li>
              <li><strong>Стресс:</strong> Постарайтесь избегать стрессовых ситуаций перед анализом.</li>
            </ul>
            <p class="mt-4"><strong>Для гематологических исследований (общий анализ крови, коагулограмма):</strong></p>
            <ul class="list-disc pl-5 space-y-2">
              <li>Особенно важно соблюдать режим голодания</li>
              <li>Избегайте обезвоживания - пейте воду перед анализом</li>
              <li>При сдаче на свертываемость (коагулограмма) сообщите о приеме антикоагулянтов</li>
            </ul>
          `
        },
        {
          id: 2,
          title: 'Гематологические заболевания: симптомы и профилактика',
          excerpt: 'Основные признаки проблем с кровью и как сохранить здоровье кроветворной системы...',
          category: 'Гематология',
          date: '15.06.2023',
          image: 'https://images.unsplash.com/photo-1576091160550-2173dba999ef?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&q=80',
          content: `
            <h4>Гематологические заболевания: симптомы и профилактика</h4>
            <p>Гематологические заболевания связаны с нарушениями в составе крови, кроветворных органах или системе свертывания крови.</p>
            
            <h5 class="font-bold mt-4">Основные симптомы:</h5>
            <ul class="list-disc pl-5 space-y-2">
              <li><strong>Слабость и усталость</strong> - могут указывать на анемию</li>
              <li><strong>Бледность кожи</strong> - признак снижения гемоглобина</li>
              <li><strong>Кровоточивость десен, носовые кровотечения</strong> - возможны при нарушениях свертываемости</li>
              <li><strong>Необъяснимые синяки</strong> - могут свидетельствовать о тромбоцитопении</li>
              <li><strong>Увеличение лимфоузлов</strong> - возможный признак лимфопролиферативных заболеваний</li>
              <li><strong>Длительное заживление ран</strong> - нарушение свертывающей системы</li>
              <li><strong>Частые инфекции</strong> - могут указывать на лейкопению</li>
            </ul>
            
            <h5 class="font-bold mt-4">Профилактика:</h5>
            <ul class="list-disc pl-5 space-y-2">
              <li>Сбалансированное питание с достаточным количеством железа, витамина B12 и фолиевой кислоты</li>
              <li>Регулярные профилактические осмотры и анализы крови</li>
              <li>Контроль хронических заболеваний, влияющих на кровь (болезни печени, почек)</li>
              <li>Избегание токсических воздействий (химикаты, радиация)</li>
              <li>Умеренная физическая активность для улучшения кровообращения</li>
              <li>Отказ от вредных привычек (курение, алкоголь)</li>
              <li>Своевременное обращение к врачу при появлении тревожных симптомов</li>
            </ul>
          `
        },
        {
          id: 3,
          title: 'Железодефицитная анемия: причины и лечение',
          excerpt: 'Все о самом распространенном гематологическом заболевании и методах его коррекции...',
          category: 'Анемия',
          date: '20.06.2023',
          image: 'https://images.unsplash.com/photo-1631815588090-d4bfec5b1ccb?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&q=80',
          content: `
            <h4>Железодефицитная анемия: причины и лечение</h4>
            <p>Железодефицитная анемия - состояние, при котором в организме недостаточно железа для производства гемоглобина.</p>
            
            <h5 class="font-bold mt-4">Основные причины:</h5>
            <ul class="list-disc pl-5 space-y-2">
              <li><strong>Кровопотери:</strong> обильные менструации, язвы ЖКТ, геморрой, донорство</li>
              <li><strong>Недостаточное поступление:</strong> несбалансированное питание, вегетарианство</li>
              <li><strong>Нарушение всасывания:</strong> заболевания ЖКТ, операции на желудке/кишечнике</li>
              <li><strong>Повышенная потребность:</strong> беременность, лактация, активный рост у детей</li>
            </ul>
            
            <h5 class="font-bold mt-4">Симптомы:</h5>
            <ul class="list-disc pl-5 space-y-2">
              <li>Слабость, быстрая утомляемость</li>
              <li>Головокружения, шум в ушах</li>
              <li>Одышка при физической нагрузке</li>
              <li>Бледность кожи и слизистых</li>
              <li>Ломкость ногтей и волос</li>
              <li>Извращение вкуса (желание есть мел, землю)</li>
              <li>Учащенное сердцебиение</li>
            </ul>
            
            <h5 class="font-bold mt-4">Лечение:</h5>
            <ul class="list-disc pl-5 space-y-2">
              <li><strong>Препараты железа:</strong> назначаются врачом, принимаются длительно (3-6 месяцев)</li>
              <li><strong>Диета:</strong> красное мясо, печень, гречка, гранаты, яблоки + витамин C для лучшего усвоения</li>
              <li><strong>Лечение причины:</strong> устранение источника кровопотери</li>
              <li><strong>В тяжелых случаях:</strong> переливание эритроцитарной массы</li>
            </ul>
            
            <p class="mt-4"><strong>Важно:</strong> Не принимайте препараты железа без назначения врача - избыток железа также опасен!</p>
          `
        }
      ],
      selectedArticle: null
    }
  },
  async created() { 
      await this.loadPatientData();
      await this.loadAppointments();
  },
  methods: {
    async loadPatientData() {
      try {
        const patientId = localStorage.getItem('patientId');
        const response = await axios.get(`https://localhost:7098/api/patient/${patientId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        this.userName = response.data.surname + " " + response.data.name + " " + response.data.patronymic;
      } catch (error) {
        if (error.response?.status === 401) {
          this.$router.push('/login');
        } else {
          console.error('Ошибка загрузки данных пациента:', error);
        }
      }
    },
    openArticleModal(article) {
      this.selectedArticle = article;
      // Блокируем прокрутку основного контента при открытом модальном окне
      document.body.style.overflow = 'hidden';
    },
    async loadAppointments() {
      try {
        const patientId = localStorage.getItem('patientId');
        const response = await axios.get(`https://localhost:7098/api/reception/user-receptions/${patientId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        
        this.nextAppointments = response.data;
        this.upcomingAppointments = this.nextAppointments.length;
      } catch (error) {
        if (error.response?.status === 401) {
          this.$router.push('/login');
        } else {
          console.error('Ошибка загрузки приемов:', error);
        }
      }
    },
    getStatusText(status) {
      const statusMap = {
        'Scheduled': 'Запланирован',
        'Completed': 'Завершен',
        'Cancelled': 'Отменен',
        'Confirmed': 'Подтвержден'
      };
      return statusMap[status] || status;
    },
    formatDate(dateString) {
      const options = { day: 'numeric', month: 'long' };
      return new Date(dateString).toLocaleDateString('ru-RU', options);
    },
    formatTime(timeString) {
      return timeString;
    },
    statusBadgeClass(status) {
      const statusClassMap = {
        'Scheduled': 'bg-yellow-100 text-yellow-800',
        'Completed': 'bg-green-100 text-green-800',
        'Cancelled': 'bg-red-100 text-red-800',
        'Confirmed': 'bg-blue-100 text-blue-800'
      };
      return statusClassMap[status] || 'bg-gray-100 text-gray-800';
    }
  },
  watch: {
    selectedArticle(newVal) {
      // Восстанавливаем прокрутку основного контента при закрытии модального окна
      if (!newVal) {
        document.body.style.overflow = '';
      }
    }
  }
}
</script>

<style>
.prose {
  line-height: 1.6;
}

.prose h4, .prose h5 {
  font-weight: 600;
  margin-top: 1.5em;
  margin-bottom: 0.5em;
}

.prose ul {
  margin-bottom: 1em;
}

.prose p {
  margin-bottom: 1em;
}

/* Стили для модального окна */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-content-enter-active,
.modal-content-leave-active {
  transition: all 0.3s ease;
}

.modal-content-enter-from,
.modal-content-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>