import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/patientViews/Login.vue';
import Register from '../views/patientViews/Register.vue';
import HemoControl from '../views/patientViews/HemoControl.vue';
import BookAppoinment from '../views/patientViews/BookAppoinment.vue';
import AppLayot from '../layout/AppLayot.vue';
import Profile from '../views/patientViews/Profile.vue';
import Appointments from '../views/patientViews/Appointments.vue';
import DoctorLayout from '../layout/DoctorLayout.vue';
import DoctorHomePage from '../views/doctorsViews/DoctorHomePage.vue';
import DoctorProfilePage from '../views/doctorsViews/DoctorProfilePage.vue';
import PersonalRegister from '../views/personalViews/PersonalRegister.vue';
import PersonalLogin from '../views/personalViews/PersonalLogin.vue';
import AssistentLayout from '../layout/AssistentLayout.vue';
import AssistentHomePage from '../views/assistentViews/AssistentHomePage.vue';
import AssistentProfilePage from '../views/assistentViews/AssistentProfilePage.vue';

const routes = [
    { 
        path: '/hemocontrol', 
        component: AppLayot, 
        name: 'AppLayot', children: [
            { path: '/hemocontrol', component: HemoControl, name: 'HemoControl', meta: { requiresAuth: true } },
            { path: '/appointments/book', component: BookAppoinment, name: 'BookAppoinment', meta: { requiresAuth: true } },
            { path: '/hemocontrol/profile', component: Profile, name: 'Profile', meta: { requiresAuth: true } },
            { path: '/hemocontrol/appointments', component: Appointments, name: 'Appointments', meta: { requiresAuth: true } },
        ]
    },
    {
      path: '/doctor',
      component: DoctorLayout,
      children: [
       { path: '/doctor/home', name: 'DoctorHomePage', component: DoctorHomePage, name: 'DoctorHomePage', meta: { requiresAuth: true } },
       { path: '/doctor/profile', name: 'DoctorProfilePage', component: DoctorProfilePage, name: 'DoctorProfilePage', meta: { requiresAuth: true } },
      ]
    },
    {
      path: '/assistent',
      component: AssistentLayout,
      children: [
       { path: '/assistent/home', name: 'AssistentHomePage', component: AssistentHomePage, name: 'AssistentHomePage', meta: { requiresAuth: true } },
       { path: '/assistent/profile', name: 'AssistentProfilePage', component: AssistentProfilePage, name: 'AssistentProfilePage', meta: { requiresAuth: true } },
      ]
    },
    { path: '/register', component: Register, name: 'Register' },
    { path: '/login', component: Login, name: 'Login' },
    { path: '/personal-register', component: PersonalRegister, name: 'PersonalRegister' },
    { path: '/personal-login', component: PersonalLogin, name: 'PersonalLogin' },
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
