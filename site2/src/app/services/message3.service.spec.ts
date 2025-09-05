import { TestBed } from '@angular/core/testing';

import { Message3Service } from './message3.service';

describe('Message3Service', () => {
  let service: Message3Service;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Message3Service);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
