import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmRfqService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetMatnr(matnr = "") {
    return this.httpClient.get(`${this.uriConstant.SrmRfq}/GetMatnr?matnr=${matnr}`);
  }
  GetVendor(vendor = "") {
    return this.httpClient.get(`${this.uriConstant.SrmRfq}/GetVendor?vendor=${vendor}`);
  }
  GetRfqData(id) {
    return this.httpClient.get(`${this.uriConstant.SrmRfq}/GetRfqData?id=${id}`)
  }
  SAVE(rfq) {
    return this.httpClient.post(`${this.uriConstant.SrmRfq}/Save`,rfq);
  }
}
