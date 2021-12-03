import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmMeasureService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetMeasureUnit() {
    return this.httpClient.post(`${this.uriConstant.SrmMeasureUnit}/GetMeasureUnit`, null);
  }
}
