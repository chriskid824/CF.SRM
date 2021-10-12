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
  AddMatnr(material){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/AddMatnr`, material);
  }
  Checkdata(query){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/Checkdata`, query);
  }
  GetEkgrp(query){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetEkgrp`, query);
  }
  GetGroupList(querylist){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetGroupList`, querylist);
  }
  GetUnitList(querylist){
    return this.httpClient.post(`${this.uriConstant.SrmMaterial}/GetUnitList`, querylist);
  }
}
