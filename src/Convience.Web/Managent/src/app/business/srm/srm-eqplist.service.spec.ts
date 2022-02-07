import { TestBed } from '@angular/core/testing';

import { SrmEqplistService } from './srm-eqplist.service';

describe('SrmEqplistService', () => {
  let service: SrmEqplistService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SrmEqplistService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
