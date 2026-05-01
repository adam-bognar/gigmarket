import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  { path: '', renderMode: RenderMode.Prerender },
  { path: 'browse', renderMode: RenderMode.Prerender },
  { path: 'login', renderMode: RenderMode.Prerender },
  { path: 'become', renderMode: RenderMode.Prerender },
  { path: 'gigs/:id', renderMode: RenderMode.Server },

  { path: 'orders', renderMode: RenderMode.Client },
  { path: 'orders/**', renderMode: RenderMode.Client },
  { path: 'dashboard/**', renderMode: RenderMode.Client },
  { path: 'inbox', renderMode: RenderMode.Client },
  { path: 'inbox/**', renderMode: RenderMode.Client },
  { path: 'account', renderMode: RenderMode.Client },
  { path: 'create-gig', renderMode: RenderMode.Client },
  { path: 'create-gig/:id', renderMode: RenderMode.Client },

  { path: '**', renderMode: RenderMode.Server },
];
