import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientChildrenComponent } from './client-children.component';

describe('ClientChildrenComponent', () => {
  let component: ClientChildrenComponent;
  let fixture: ComponentFixture<ClientChildrenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ClientChildrenComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ClientChildrenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
