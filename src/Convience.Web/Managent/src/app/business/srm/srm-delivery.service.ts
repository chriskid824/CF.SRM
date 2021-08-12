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
  GetDeliveryL(query) {
    return this.httpClient.get(`${this.uriConstant.SrmDelivery}/GetDeliveryL?DeliveryLId=${query.deliveryLId}&DeliveryNum=${query.deliveryNum}`);
  }
  AddDelivery(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDelivery}/AddDelivery`,po);
  }
}
