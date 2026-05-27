import {ApplicationConfig, inject, PLATFORM_ID, provideAppInitializer, provideBrowserGlobalErrorListeners} from '@angular/core';
import {provideRouter} from '@angular/router';
import {isPlatformBrowser} from '@angular/common';

import {routes} from './app.routes';
import {provideClientHydration, withEventReplay} from '@angular/platform-browser';
import {provideHttpClient, withFetch, withInterceptors} from '@angular/common/http';
import {authInterceptor} from './shared/interceptors/auth.interceptor';
import {AuthService} from './shared/services/auth.service';
import {catchError, firstValueFrom, of, timeout} from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    provideAppInitializer(() => {
      const platformId = inject(PLATFORM_ID);
      if (!isPlatformBrowser(platformId)) {
        return;
      }

      return firstValueFrom(
        inject(AuthService)
          .getMe()
          .pipe(
            timeout(5000),
            catchError(() => of(null))
          )
      );
    }),
  ]
};
