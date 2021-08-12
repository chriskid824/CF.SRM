import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmDeliveryService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetDelivery(query) {
    return this.httpClient.post(`${this.uriConstant.SrmDelivery}/GetDelivery`,query);
  }
  GetDeliveryL(data) {
    return this.httpClient.get(`${this.uriConstant.SrmPo}/GetDeliveryL?DeliveryId=${data.DeliveryId}&DeliveryLId=${data.DeliveryLId}`);
  }
  AddDelivery(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDelivery}/AddDelivery`,po);
  }
}
