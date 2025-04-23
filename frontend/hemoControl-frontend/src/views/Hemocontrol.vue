<template>
  <div class="flex min-h-screen bg-blue-50 text-black">
    <!-- Контент -->
    <div class="flex-1 flex flex-col">
      <!-- Основной контент -->
      <main class="p-6 flex flex-col space-y-6">
        <!-- Ближайший приём -->
        <div class="bg-white rounded-lg shadow p-6 flex space-x-4 items-start">
          <i data-feather="calendar" class="text-blue-600 text-3xl"></i>
          <div>
            <h3 class="text-xl font-semibold">Ближайший приём</h3>
            <p class="text-gray-700 mt-1">15 апреля 2025, 14:00</p>
            <p class="text-sm text-gray-500">Гематолог Иванов И.И. — Кабинет 210</p>
          </div>
        </div>

        <!-- Последние анализы -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-xl font-semibold mb-3 flex items-center space-x-2">
            <i data-feather="file-text" class="text-blue-600"></i><span>Результаты анализов</span>
          </h3>
          <ul class="text-base text-gray-700 space-y-1">
            <li>Гемоглобин: 128 г/л (норма)</li>
            <li>Лейкоциты: 4.8 x10⁹/л (норма)</li>
            <li>Тромбоциты: 90 x10⁹/л <span class="text-red-500">(понижены)</span></li>
          </ul>
        </div>

        <!-- Уведомления -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-xl font-semibold mb-3 flex items-center space-x-2">
            <i data-feather="bell" class="text-blue-600"></i><span>Уведомления</span>
          </h3>
          <ul class="text-base text-gray-700 space-y-1">
            <li>💬 Новое сообщение от врача</li>
            <li>📄 Добавлен новый документ</li>
            <li>⏰ Напоминание: приём завтра в 14:00</li>
          </ul>
        </div>

        <!-- Документы -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-xl font-semibold mb-3 flex items-center space-x-2">
            <i data-feather="folder" class="text-blue-600"></i><span>Документы</span>
          </h3>
          <ul class="text-base text-gray-700 space-y-1">
            <li>Справка для работы</li>
            <li>Выписка из стационара</li>
            <li>Направление на КТ</li>
          </ul>
        </div>

        <!-- Полезный совет -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-xl font-semibold mb-3 flex items-center space-x-2">
            <i data-feather="info" class="text-blue-600"></i><span>Совет дня</span>
          </h3>
          <p class="text-base text-gray-700">Перед сдачей крови избегайте тяжёлой еды и стрессов. Приходите натощак.</p>
        </div>
      </main>
    </div>
  </div>
</template>

<script>
import { onMounted, nextTick } from 'vue';
import feather from 'feather-icons';

export default {
  data() {
    return {
      isLoggedIn: false,
      isSidebarCollapsed: false,
      menuItems: [
        { title: 'Запись к врачу', icon: 'calendar', href: '/appointments/book' },
        { title: 'Мои приёмы', icon: 'clock', href: '/appointments' },
        { title: 'Результаты анализов', icon: 'file-text', href: '/lab-results' },
        { title: 'История болезни', icon: 'clipboard', href: '/history' },
        { title: 'Медицинская карта', icon: 'book-open', href: '/card' },
        { title: 'Документы', icon: 'folder', href: '/documents' },
        { title: 'Сообщения врачу', icon: 'message-circle', href: '/messages' },
        { title: 'Уведомления', icon: 'bell', href: '/notifications' },
      ],
    };
  },
  methods: {
    toggleSidebar() {
      this.isSidebarCollapsed = !this.isSidebarCollapsed;
      nextTick(() => feather.replace());
    },
    goToLogin() {
      this.$router.push('/login');
    },
    goToProfile() {
      this.$router.push('/profile');
    },
    logout() {
      localStorage.removeItem('token');
      this.isLoggedIn = false;
      this.$router.push('/login');
    },
    checkAuth() {
      const token = localStorage.getItem('token');
      if (!token) {
        this.$router.push('/login');
        alert('Вы не авторизованы. Пожалуйста, войдите.');
      }
      this.isLoggedIn = !!token;
    },
  },
  mounted() {
    this.checkAuth();
    nextTick(() => feather.replace());
  },
};
</script>

<style scoped>
.rotate-180 {
  transform: rotate(180deg);
  transition: transform 0.3s ease;
}
</style>
