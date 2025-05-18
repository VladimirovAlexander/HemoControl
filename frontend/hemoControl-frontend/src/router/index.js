import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/Login.vue';
import Register from '../views/Register.vue';
import HemoControl from '../views/HemoControl.vue';
import DoctorDashboard from '../views/doctorsViews/DoctorDashboard.vue'; 
import PatientList from '../views/doctorsViews/PatientList.vue';
import LabTests from '../views/doctorsViews/LabTests.vue';
import BookAppoinment from '../views/patientViews/BookAppoinment.vue';
import AppLayot from '../layout/AppLayot.vue';
import Profile from '../views/Profile.vue';

const routes = [
    { 
        path: '/hemocontrol', 
        component: AppLayot, 
        name: 'AppLayot', children: [
            { path: '/hemocontrol', component: HemoControl, name: 'HemoControl', meta: { requiresAuth: true } },
            { path: '/appointments/book', component: BookAppoinment, name: 'BookAppoinment', meta: { requiresAuth: true } },
            { path: '/doctor/dashboard', component: DoctorDashboard, name: 'DoctorDashboard', meta: { requiresAuth: true } },
            { path: '/doctor/patients', component: PatientList, name: 'PatientList', meta: { requiresAuth: true } },
            { path: '/doctor/lab-requests', component: LabTests, name: 'LabTests', meta: { requiresAuth: true } },
            { path: '/hemocontrol/profile', component: Profile, name: 'Profile', meta: { requiresAuth: true } },
        ]
    },
    { path: '/register', component: Register, name: 'Register' },
    { path: '/login', component: Login, name: 'Login' },
    { path: '/', redirect: { name: 'Login' } },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});


router.beforeEach((to, from, next) => {
    const isLoggedIn = !!localStorage.getItem('token');
  
    if (to.meta.requiresAuth && !isLoggedIn) {
      next({
        name: 'Login',
        query: { message: 'Вы должны войти, чтобы получить доступ к этой странице.' },
      });
    } else {
      next();
    }
  });

export default router;
