import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmMaterialService {
  constructor(private httpClient: HttpClient,private uriConstant: UriConfig){ }
  
  GetMaterialList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetMaterialList`, query);
  }
  GetMaterialDetail(query) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetMaterialDetail`, query);
  }    
  update(material) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/UpdateMaterial`, material);
  }
  CheckMatnr(query){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/CheckMatnr`, query);
  }
  AddMatnr(material){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/AddMatnr`, material);
  }
  UploadFile(formData) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/UploadFile`, formData);
  }
  GetMaterialTrendList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetMaterialTrendList`, query);
  }
}
