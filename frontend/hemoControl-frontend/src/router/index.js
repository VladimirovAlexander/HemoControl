import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/Login.vue';
import Register from '../views/Register.vue';
import HemoControl from '../views/HemoControl.vue';
import DoctorDashboard from '../views/doctorsViews/doctorDashboard.vue'; // Импортируем новую страницу

const routes = [
    { path: '/', redirect: { name: 'Login' } },
    { path: '/login', component: Login, name: 'Login' },
    { path: '/register', component: Register, name: 'Register' },
    { path: '/hemocontrol', component: HemoControl, name: 'HemoControl' },
    { path: '/doctor/dashboard', component: DoctorDashboard, name: 'DoctorDashboard' }, // Новая страница
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;
