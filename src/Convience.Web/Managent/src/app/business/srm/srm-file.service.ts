import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UriConfig } from 'src/app/configs/uri-config';
import { Observable } from 'dist/manage/tinymce/tinymce';
import { map, filter, switchMap } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})

export class SrmFileService {
  constructor(private httpClient: HttpClient,
    private uriConstant: UriConfig) { }

  GetTemplateList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/GetTemplateList`,query);
  }
  GetFileList(query) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/GetFileList`,query);
  }
  GetAnnList() {
    return this.httpClient.get(`${this.uriConstant.SrmFile}/GetAnnList`);
  }
  GetDeliveryL(query) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/GetDeliveryL`,query);
  }
  AddTemplate(po) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/AddTemplate`,po);
  }
  UpdateTemplate(po) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/UpdateTemplate`,po);
  }
  Upload(po) {
    return this.httpClient.post(`${this.uriConstant.SrmFile}/UploadFile`,po);
  }
  download(uid,Name) {
    let uri = `${this.uriConstant.SrmFile}?uid=${uid}&&Name=${Name}`;
    return this.httpClient.get(uri, { responseType: "blob", });
  }
  get(page, size, directory) {
    let uri = `${this.uriConstant.SrmFile}/list?page=${page}&&size=${size}&&directory=${directory}`;
    return this.httpClient.get(uri);
  }
  delete(uid) {
    let uri = `${this.uriConstant.SrmFile}?uid=${uid}`;
    return this.httpClient.delete(uri);
  }

//   postFile(fileToUpload: File): Observable<boolean> {
//     const endpoint = 'your-destination-url';
//     const formData: FormData = new FormData();
//     formData.append('fileKey', fileToUpload, fileToUpload.name);
//     return this.httpClient
//       .post(endpoint, formData, { headers: yourHeadersConfig })
//       .map(() => { return true; })
//       .catch((e) => this.handleError(e));
// }
}
