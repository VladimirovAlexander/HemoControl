<template>
  <div class="bg-blue-50">
    <!-- Иконки над навигационным меню -->
    <div class="bg-white text-black flex items-center justify-between py-4 px-8">
        <h1 class="text-xl font-bold">
            <span class="text-red-500 text-2xl">Гемо</span>
            <span class="text-blue-600 text-2xl">контроль</span>
        </h1>

        <!-- Если пользователь авторизован, показываем иконку профиля, если нет - кнопку Войти -->
        <div v-if="isLoggedIn">
            <img src="" alt="Профиль" class="rounded-full w-8 h-8 cursor-pointer" @click="goToProfile">
        </div>
        <button v-else @click="goToLogin" class="text-1xl bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
            Войти
        </button>
    </div>

    <!-- Навигационное меню -->
    <nav class="bg-blue-600 p-4 text-white flex justify-center">
        <div class="flex space-x-10">
            <a
                v-for="item in menuItems"
                :key="item.title"
                :href="item.href"
                class="flex flex-col items-center justify-center text-center w-24 h-24 hover:underline"
            >
                <i :data-feather="item.icon" class="mb-2 text-2xl"></i>
                <span class="text-sm h-6 leading-snug text-center break-words w-full">
                    {{ item.title }}
                </span>
            </a>
        </div>
    </nav>

    <!-- Раздел поиска -->
    <div class="p-8 bg-blue-500 text-white text-center">
        <input
            v-model="searchQuery"
            type="text"
            placeholder="Введите запрос..."
            class="w-1/2 p-2 rounded-md"
        />
    </div>

    <!-- Основной контент -->
    <div class="grid grid-cols-2 gap-4 p-8">
        <div
            v-for="item in menuItems"
            :key="item.title"
            class="shadow hover:shadow-lg transition bg-white p-6 rounded flex items-center space-x-4"
        >
            <i :data-feather="item.icon" class="text-blue-500"></i>
            <div>
                <h2 class="text-xl font-semibold">{{ item.title }}</h2>
                <p class="text-gray-600">Быстрый доступ к разделу.</p>
            </div>
        </div>
    </div>
  </div>
</template>

<script>
import feather from 'feather-icons';

export default {
  data() {
    return {
      searchQuery: "",  // Значение для поисковой строки
      isLoggedIn: false, // Флаг, который отслеживает, авторизован ли пользователь
      menuItems: [
        { title: "История болезни", icon: "clipboard", href: "#" },
        { title: "Медицинская карта", icon: "book-open", href: "#" },
        { title: "Календарь симптомов", icon: "calendar", href: "#" },
        { title: "Уведомления", icon: "bell", href: "#" },
        { title: "Моя аптека", icon: "box", href: "#" },
        { title: "Аналитика", icon: "pie-chart", href: "#" },
        { title: "База знаний", icon: "info", href: "#" },
        { title: "Инструкции", icon: "book-open", href: "#" },
      ],
    };
  },
  methods: {
    // Метод для поиска
    search() {
      alert(`Поиск по запросу: ${this.searchQuery}`);
    },

    // Переход на страницу авторизации
    goToLogin() {
      this.$router.push('/login');
      // Логика авторизации, например:
      this.isLoggedIn = true;  // Здесь можно изменить на реальную логику авторизации
    },

    // Переход на страницу профиля
    goToProfile() {
      this.$router.push('/profile');
    }
  },
  mounted() {
    // Инициализация иконок с помощью библиотеки Feather
    if (typeof feather !== 'undefined') {
      feather.replace(); // Если библиотека Feather загружена, инициализируем её
    } else {
      console.error("Feather icons не загружены!");
    }
  },
};
</script>

<style scoped>
body {
  font-family: 'Arial', sans-serif;
}

input {
  transition: all 0.3s ease;
}

input:focus {
  border-color: #3b82f6;
  outline: none;
}

button {
  transition: all 0.3s ease;
}

button:hover {
  background-color: #3b82f6;
  color: white;
}
</style>
