import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmPoService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetPo(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/GetPo`,query);
  }
  GetPoL(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/GetPoL`,query);
  }
  GetPoPoL(query) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/GetPoPoL`,query);
  }
  SAVE(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/Save`,po);
  }
  GetPoDoc(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/Sap_GetPoDoc`,po);
  }
  UpdateReplyDeliveryDate(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/UpdateReplyDeliveryDate`,po);
  }
  UpdateStatus(id) {
    return this.httpClient.get(`${this.uriConstant.SrmPo}/UpdateStatus?id=${id}`);
  }
  StartUp(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/StartUp`, po);
  }
  Cancel(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/Cancel`, po);
  }
  Delete(po) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/Delete`, po);
  }
  GetSourcerList(user) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/GetSourcerList`, user);
  }

  Sap_GetPoData(data) {
    return this.httpClient.post(`${this.uriConstant.SrmPo}/Sap_GetPoData`, data);
  }
  DownloadFileUrl(fileName) {
    let uri = `${this.uriConstant.SrmPo}/DownloadFileUrl?file_name=${fileName}`;
    return this.httpClient.get(uri);
  }
  DownloadFilePath(path,fileName) {
    let uri = `${this.uriConstant.SrmPo}/DownloadFilePath?path=${path}&file_name=${fileName}`;
    return this.httpClient.get(uri,{ responseType: 'blob' });
  }
}
