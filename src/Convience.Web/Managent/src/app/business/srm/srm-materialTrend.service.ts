import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmMaterialTrendService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }
  UploadFile(formData) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterialTrend}/UploadFile`, formData);
  }
  GetMaterialTrendList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterialTrend}/GetMaterialTrendList`, query);
  }
}
