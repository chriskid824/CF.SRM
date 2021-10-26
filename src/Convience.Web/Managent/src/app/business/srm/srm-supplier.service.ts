import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';
@Injectable({
  providedIn: 'root'
})

export class SrmSupplierService {
  constructor(private httpClient: HttpClient,private uriConstant: UriConfig){ }
  
  GetSupplierList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/GetSupplierList`, query);
  }    
  GetSupplierDetail(query) {
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/GetSupplierDetail`, query);
  }    
  update(supplier) {
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/UpdateSupplier`, supplier);
  }
  Checkdata(query){
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/Checkdata`, query);
  }
  AddSupplier(supplier){
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/AddSupplier`, supplier);
  }
  BatchUpload(formData) {
    return this.httpClient.post(`${this.uriConstant.SrmSupplier}/BatchUpload`, formData);
  }
}
