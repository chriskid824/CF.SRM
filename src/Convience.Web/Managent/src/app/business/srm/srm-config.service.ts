import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmConfigService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }
  GetProcessList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmConfig}/GetProcessList`, query);
  }
  AddProcess(process) {
    return this.httpClient.post(`${this.uriConstant.SrmConfig}/AddProcess`, process);
  }
  GetSurfaceList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmConfig}/GetSurfaceList`, query);
  }
  AddSurface(surface) {
    return this.httpClient.post(`${this.uriConstant.SrmConfig}/AddSurface`, surface);
  }
}

