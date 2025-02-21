import {createRouter,createWebHashHistory, createWebHistory} from 'vue-router';
import Login from '../views/Login.vue';
import Register from '../views/Register.vue';
import HemoControl from '../views/HemoControl.vue';

const routes = [
    { path: '/', redirect: { name: 'Login' } },
    { path: '/login', component: Login, name: 'Login' },
    { path: '/register', component: Register, name: 'Register' },
    { path: '/hemocontrol', component: HemoControl, name: 'HemoControl' },
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;