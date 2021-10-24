import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmFileService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetTemplateList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/GetTemplateList`,query);
  }
  GetDeliveryL(query) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/GetDeliveryL`,query);
  }
  AddTemplate(po) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/AddTemplate`,po);
  }
  UpdateTemplate(po) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/UpdateTemplate`,po);
  }
}
