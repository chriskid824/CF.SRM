import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})
export class SrmEqpService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }
    GetPoData(query) {
      return this.httpClient.post(`${this.uriConstant.SrmEqp}/GetPoData`,query);
    }
    GetEqpList(query) {
      return this.httpClient.post(`${this.uriConstant.SrmEqp}/GetEqpList`, query);
    }
    SAVE(eqp) {
      return this.httpClient.post(`${this.uriConstant.SrmEqp}/Save`,eqp);
    }
    Delete(eqp) {
      return this.httpClient.post(`${this.uriConstant.SrmEqp}/Delete`,eqp);
    }
    Start(eqp) {
      return this.httpClient.post(`${this.uriConstant.SrmEqp}/Start`, eqp);
    }
    GetEqpData(id) {
      return this.httpClient.get(`${this.uriConstant.SrmEqp}/GetEqpData?id=${id}`)
    }
}
