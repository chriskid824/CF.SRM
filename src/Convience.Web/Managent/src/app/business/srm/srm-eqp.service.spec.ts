import { TestBed } from '@angular/core/testing';

import { SrmEqpService } from './srm-eqp.service';

describe('SrmEqpService', () => {
  let service: SrmEqpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SrmEqpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
