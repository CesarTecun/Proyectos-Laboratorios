import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaliComponent } from './cali.component';

describe('CaliComponent', () => {
  let component: CaliComponent;
  let fixture: ComponentFixture<CaliComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CaliComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CaliComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
