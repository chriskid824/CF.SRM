import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';

@Injectable({
  providedIn: 'root'
})

export class SrmDisscussionService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetDissList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/GetDissList`,query);
  }
  GetDisscussion(query) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/GetDisscussion`,query);
  }
  GetNumberList(id) {
    let uri = `${this.uriConstant.SrmDisscussion}/GetNumberList?id=${id}`;
    return this.httpClient.get(uri);
  }
  AddTitle(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/AddTitle`,po);
  }
  AddContent(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/AddContent`,po);
  }
  UpdateContent(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/UpdateContent`,po);
  }
  DeleteContent(po) {
    return this.httpClient.post(`${this.uriConstant.SrmDisscussion}/DeleteContent`,po);
  }
  get(page, size, directory) {
    let uri = `${this.uriConstant.SrmDisscussion}/list?page=${page}&&size=${size}&&directory=${directory}`;
    return this.httpClient.get(uri);
  }
  delete(uid) {
    let uri = `${this.uriConstant.SrmDisscussion}?uid=${uid}`;
    return this.httpClient.delete(uri);
  }
}
