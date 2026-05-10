import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Pricing } from './pricing';

describe('Pricing', () => {
  let component: Pricing;
  let fixture: ComponentFixture<Pricing>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Pricing],
    }).compileComponents();

    fixture = TestBed.createComponent(Pricing);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start with three enabled packages', () => {
    expect(component.packagesEnabled()).toBe(true);
    expect(component.visiblePackages().length).toBe(3);
    expect(component.packageSectionTitle()).toBe('Offer Packages');
  });

  it('should switch to single package mode when packages are disabled', () => {
    component.togglePackages();

    expect(component.packagesEnabled()).toBe(false);
    expect(component.visiblePackages().length).toBe(1);
    expect(component.visiblePackages()[0].label).toBe('Basic');
    expect(component.packageSectionTitle()).toBe('Offer a Single Package');
  });

  it('should detect invalid package name', () => {
    const pkg = component.visiblePackages()[0];

    expect(component.isNameInvalid({ ...pkg, name: 'ab' })).toBe(true);
    expect(component.isNameInvalid({ ...pkg, name: 'Valid package name' })).toBe(false);
  });

  it('should detect invalid package description', () => {
    const pkg = component.visiblePackages()[0];

    expect(component.isDescriptionInvalid({ ...pkg, description: 'too short' })).toBe(true);
    expect(
      component.isDescriptionInvalid({
        ...pkg,
        description: 'This is a long enough package description.',
      }),
    ).toBe(false);
  });

  it('should detect invalid price', () => {
    const pkg = component.visiblePackages()[0];

    expect(component.isPriceInvalid({ ...pkg, price: null })).toBe(true);
    expect(component.isPriceInvalid({ ...pkg, price: 4 })).toBe(true);
    expect(component.isPriceInvalid({ ...pkg, price: 5 })).toBe(false);
  });

  it('should not emit continue event when a visible package is invalid', () => {
    const emitSpy = vi.spyOn(component.continue, 'emit');

    component.updateName(0, {
      target: {value: 'ab'},
    } as unknown as Event);

    component.submit();

    expect(component.showErrors()).toBe(true);
    expect(emitSpy).not.toHaveBeenCalled();
  });

  it('should emit pricing payload when packages are valid', () => {
    const emitSpy = vi.spyOn(component.continue, 'emit');

    component.submit();

    expect(emitSpy).toHaveBeenCalledOnce();

    const emitted = emitSpy.mock.calls[0][0];

    expect(emitted.packages.length).toBe(3);
    expect(emitted.packages[0]).toEqual({
      tier: 'Basic',
      name: 'Basic Starter',
      description: 'Core service features for a quick start.',
      deliveryDays: 2,
      revisions: 1,
      price: 20,
    });
    expect(emitted.packages[2].revisions).toBe(999);
  });

  it('should emit only the basic package in single package mode', () => {
    const emitSpy = vi.spyOn(component.continue, 'emit');

    component.togglePackages();
    component.submit();

    const emitted = emitSpy.mock.calls[0][0];

    expect(emitted.packages.length).toBe(1);
    expect(emitted.packages[0].tier).toBe('Basic');
  });
});
