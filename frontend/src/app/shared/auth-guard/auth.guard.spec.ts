import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
  function runGuard() {
    return TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));
  }

  it('should allow navigation when user is authenticated', () => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: {
            isAuthenticated: () => true,
          },
        },
        {
          provide: Router,
          useValue: {
            createUrlTree: vi.fn(),
          },
        },
      ],
    });

    expect(runGuard()).toBe(true);
  });

  it('should redirect to login when user is not authenticated', () => {
    const loginTree = {} as UrlTree;
    const router = {
      createUrlTree: vi.fn().mockReturnValue(loginTree),
    };

    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: {
            isAuthenticated: () => false,
          },
        },
        {
          provide: Router,
          useValue: router,
        },
      ],
    });

    expect(runGuard()).toBe(loginTree);
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login']);
  });
});
