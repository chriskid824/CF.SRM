import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmPriceService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }
  GetQotDetail(query) {
    return this.httpClient.post(`${this.uriConstant.SrmQot}/GetQotDetail`, query);
  }
}
