import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})
export class SrmQotService {

  constructor(private httpClient: HttpClient,
  private uriConstant: UriConfig) { }
  Save(qot) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/Save`, qot);
  }
  GetQotList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetQotList`,query);
  }
  GetQotData(id,rfqid,vendorId) {
    //return this.httpClient.get(`${this.uriConstant.SrmQot}/GetQotData?id=${id}`)
    return this.httpClient.get(`${this.uriConstant.SrmQot}/GetQotData?id=${id}&rfqid=${rfqid}&vendorid=${vendorId}`)
  }
  GetQotDetail(query) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetQotInfo`, query);
    //return this.httpClient.post(`${this.uriConstant.SrmQot}/GetQotList`,query);
  } 
  Reject(qot) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/Reject`, qot);
  }
  Send(qot) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/Send`, qot);
  }
  GetRowNum(qot) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetRowNum`,qot );
  }
  GetProcess() {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetProcess`, null);
  }
  GetMaterial() {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetMaterial`, null);
  }
}
