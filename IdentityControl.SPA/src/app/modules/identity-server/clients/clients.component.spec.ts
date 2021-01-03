import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiScopesComponent } from './api-scopes.component';

describe('ApiScopesComponent', () => {
  let component: ApiScopesComponent;
  let fixture: ComponentFixture<ApiScopesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ApiScopesComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApiScopesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
