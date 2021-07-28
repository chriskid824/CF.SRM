import { TestBed } from '@angular/core/testing';

import { QotApiService } from './qot-api.service';

describe('QotApiService', () => {
  let service: QotApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QotApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
