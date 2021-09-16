import { TestBed } from '@angular/core/testing';

import { SrmSupplierService } from './srm-supplier.service';

describe('SrmSupplierService', () => {
  let service: SrmSupplierService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SrmSupplierService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
