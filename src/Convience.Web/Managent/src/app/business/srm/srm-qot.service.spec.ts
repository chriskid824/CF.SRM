import { TestBed } from '@angular/core/testing';

import { SrmQotService } from './srm-qot.service';

describe('SrmQotService', () => {
  let service: SrmQotService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SrmQotService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
