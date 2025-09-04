import { TestBed } from '@angular/core/testing';

import { Message2Service } from './message2.service';

describe('Message2Service', () => {
  let service: Message2Service;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Message2Service);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
