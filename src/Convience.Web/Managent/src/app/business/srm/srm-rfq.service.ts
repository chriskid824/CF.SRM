import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmRfqService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetMatnr(matnrQuery) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/GetMatnr`, matnrQuery);
  }
  GetVendor(vendorQuery) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/GetVendor`, vendorQuery);
  }
  GetRfqData(id) {
    return this.httpClient.get(`${this.uriConstant.SrmRfq}/GetRfqData?id=${id}`)
  }
  GetRfqList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/GetRfqList`, query);
  }
  SAVE(rfq) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/Save`,rfq);
  }
  StartUp(rfq) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/StartUp`, rfq);
  }
  Cancel(rfq) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/Cancel`, rfq);
  }
  Delete(rfq) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/Delete`, rfq);
  }
  GetSourcerList(user) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/GetSourcerList`, user);
  }
  GetRfq(rfqH) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/GetRfq`, rfqH);
  }
}
