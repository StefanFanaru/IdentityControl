import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavbarSubitemComponent } from './navbar-subitem.component';

describe('NavbarSubitemComponent', () => {
  let component: NavbarSubitemComponent;
  let fixture: ComponentFixture<NavbarSubitemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NavbarSubitemComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavbarSubitemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
