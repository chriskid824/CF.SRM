import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})
export class QotApiService {

  constructor(
    private http: HttpClient,
    private uriConstant: UriConfig) { }
  apicall() {
    return this.http.get('https://hsuchihting.github.io/angular/20210307/1877603814/');
  }
  SaveQotMatnr(qot) {
    return this.http.post(`${this.uriConstant.SrmQot}/Save`, qot);
  }
}
