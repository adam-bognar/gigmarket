import {Routes} from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/landing/landing').then((m) => m.Landing),
  },
  {
    path: 'browse',
    loadComponent: () => import('./pages/browse/browse').then((m) => m.Browse),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  {
    path: 'become',
    loadComponent: () => import('./pages/become-a-freelancer/become-a-freelancer').then((m) => m.BecomeAFreelancer),
  },
  {
    path: 'create-gig',
    loadComponent: () => import('./pages/create-gig/create-gig').then((m) => m.CreateGig),
  },
  {
    path: 'create-gig/:id',
    loadComponent: () => import('./pages/create-gig/create-gig').then((m) => m.CreateGig),
  },
  {
    path: 'gigs/:id',
    loadComponent: () => import('./pages/gig-details/gig-details').then((m) => m.GigDetails),
  },
  {
    path: 'dashboard/seller/manage-gigs',
    loadComponent: () => import('./pages/seller-dashboard/manage-gigs/manage-gigs').then((m) => m.ManageGigs),
  },
  {
    path: 'dashboard/seller/profile',
    loadComponent: () => import('./pages/seller-profile-edit/seller-profile-edit').then((m) => m.SellerProfileEdit),
  },
  {
    path: 'orders',
    loadComponent: () => import('./pages/my-orders/my-orders').then((m) => m.MyOrders),
  },
  {
    path: 'orders/success',
    loadComponent: () => import('./pages/order-success/order-success').then((m) => m.OrderSuccess),
  },
  {
    path: 'orders/:id',
    loadComponent: () => import('./pages/order-detail/order-detail').then((m) => m.OrderDetail),
  },
  {
    path: 'inbox',
    loadComponent: () => import('./pages/inbox/inbox').then((m) => m.InboxPage),
  },
  {
    path: 'inbox/:conversationId',
    loadComponent: () => import('./pages/inbox/conversation/conversation').then((m) => m.ConversationPage),
  },
  // {
  //   path: 'gigs/c2c84ec0-cd3c-4227-b627-29debdeeff45',
  //   loadComponent: () => import('./pages/gig-details/gig-details').then((m) => m.GigDetails),
  // },
];
