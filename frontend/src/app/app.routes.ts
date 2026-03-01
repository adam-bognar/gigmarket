import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./pages/landing/landing').then((m) => m.Landing),
    },
    {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then((m) => m.Login),
    },
    {
      path: 'become',
      loadComponent: () => import('./pages/become-a-freelancer/become-a-freelancer').then((m) => m.BecomeAFreelancer),
    },
];
