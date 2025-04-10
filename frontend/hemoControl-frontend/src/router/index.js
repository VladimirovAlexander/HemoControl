import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/Login.vue';
import Register from '../views/Register.vue';
import HemoControl from '../views/HemoControl.vue';
import DoctorDashboard from '../views/doctorsViews/DoctorDashboard.vue'; 
import PatientList from '../views/doctorsViews/PatientList.vue';
import LabTests from '../views/doctorsViews/LabTests.vue';


const routes = [
    { path: '/', redirect: { name: 'Login' } },
    { path: '/login', component: Login, name: 'Login' },
    { path: '/register', component: Register, name: 'Register' },
    { path: '/hemocontrol', component: HemoControl, name: 'HemoControl' },
    { path: '/doctor/dashboard', component: DoctorDashboard, name: 'DoctorDashboard' }, // Новая страница
    { path: '/doctor/patients', component: PatientList, name: 'PatientList'},
    { path: '/doctor/lab-requests', component: LabTests, name: 'LabTests'},
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;
