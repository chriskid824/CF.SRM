import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmPriceService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }
  GetQotDetail(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/GetQotDetail`, query);
  }
  Start(obj) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/Start`, obj);
  }
  Save(obj) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/Save`, obj);
  }
  GetSummary(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/GetSummary`, query);
  }
  GetTaxcodes() {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/GetTaxcodes`,null);
  }
  GetCurrency() {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/GetCurrency`, null);
  }
  GetEkgry(werks) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/GetEkgry`, werks);
  }
  QueryInfoRecord(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPrice}/QueryInfoRecord`, query);
  }
}
