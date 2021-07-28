import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})
export class SrmQotService {

   constructor(private httpClient: HttpClient,
   private uriConstant: UriConfig) { }
   SaveQotMatnr(qot) {
     return this.httpClient.post(`${this.uriConstant.SrmQot}/Save`, qot);
  }
}
