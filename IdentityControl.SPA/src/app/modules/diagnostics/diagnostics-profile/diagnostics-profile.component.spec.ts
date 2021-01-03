import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiagnosticsProfileComponent } from './diagnostics-profile.component';

describe('DiagnosticsProfileComponent', () => {
  let component: DiagnosticsProfileComponent;
  let fixture: ComponentFixture<DiagnosticsProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DiagnosticsProfileComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DiagnosticsProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
