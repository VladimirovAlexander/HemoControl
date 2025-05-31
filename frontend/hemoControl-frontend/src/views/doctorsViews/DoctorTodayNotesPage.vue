<template>
  <div class="max-w-4xl mx-auto p-6 space-y-6">
    <!-- Заголовок -->
    <div class="text-center">
      <h1 class="text-2xl font-bold text-gray-800">Заметки на сегодня</h1>
      <p class="text-gray-500 mt-1">{{ today }}</p>
    </div>

    <!-- Добавление новой заметки -->
    <div class="bg-white shadow rounded-xl p-4 flex gap-4 items-start">
      <textarea
        v-model="newNote"
        placeholder="Введите новую заметку..."
        class="form-input w-full resize-none h-24"
      ></textarea>
      <button
        @click="addNote"
        class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
      >
        Добавить
      </button>
    </div>

    <!-- Список заметок -->
    <div class="space-y-4">
      <div
        v-for="note in notes"
        :key="note.id"
        class="bg-white shadow rounded-xl p-4 flex justify-between items-start"
      >
        <div class="w-full">
          <textarea
            v-model="note.text"
            class="form-input w-full resize-none"
            rows="3"
            @blur="updateNote(note)"
          ></textarea>
        </div>
        <button
          @click="deleteNote(note.id)"
          class="text-red-500 hover:text-red-700 ml-4"
        >
          ✕
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'DoctorTodayNotesPage',
  data() {
    return {
      newNote: '',
      notes: [],
      today: new Date().toLocaleDateString('ru-RU'),
    };
  },
  methods: {
    async fetchNotes() {
      try {
        const res = await axios.get(`https://localhost:7000/api/doctor/notes/today`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        this.notes = res.data;
      } catch (err) {
        console.error('Ошибка загрузки заметок:', err);
      }
    },
    async addNote() {
      if (!this.newNote.trim()) return;
      try {
        const res = await axios.post(
          `https://localhost:7000/api/doctor/notes`,
          { text: this.newNote },
          { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } }
        );
        this.notes.push(res.data);
        this.newNote = '';
      } catch (err) {
        console.error('Ошибка добавления заметки:', err);
      }
    },
    async updateNote(note) {
      try {
        await axios.put(
          `https://localhost:7000/api/doctor/notes/${note.id}`,
          { text: note.text },
          { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } }
        );
      } catch (err) {
        console.error('Ошибка обновления заметки:', err);
      }
    },
    async deleteNote(id) {
      try {
        await axios.delete(`https://localhost:7000/api/doctor/notes/${id}`, {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        this.notes = this.notes.filter((n) => n.id !== id);
      } catch (err) {
        console.error('Ошибка удаления заметки:', err);
      }
    },
  },
  mounted() {
    this.fetchNotes();
  },
};
</script>

<style scoped>
.form-input {
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  padding: 0.5rem;
  outline: none;
  transition: border-color 0.2s;
}
.form-input:focus {
  border-color: #3b82f6;
}
</style>
