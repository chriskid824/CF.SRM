import { Component, OnInit } from '@angular/core';
import { FormBuilder,FormGroup } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';

@Component({
  selector: 'app-material-batch-upload',
  templateUrl: './material-batch-upload.component.html',
  styleUrls: ['./material-batch-upload.component.less']
})
export class MaterialBatchUploadComponent implements OnInit {
  uploadForm: FormGroup = new FormGroup({});
  fileList: any[] = [];
  uploading: boolean = false;
  currentDirectory: string = 'matnr-batch-upload';
  constructor(private _messageService: NzMessageService,
    private _srmMaterialService: SrmMaterialService,
    private _formBuilder: FormBuilder,  ) { }

  ngOnInit(): void {
    this.uploadForm = this._formBuilder.group({
    });
  }
  download() {
    //this._srmRfqService.downloadUploadExample().subscribe((result: any) => {
    //  const a = document.createElement('a');
    //  const blob = new Blob([result], { 'type': "application/octet-stream" });
    //  a.href = URL.createObjectURL(blob);
    //  a.download = fileInfo.fileName;
    //  a.click();
    //});
  }
  beforeUpload = (file): boolean => {
    this.fileList = [];
    this.fileList = this.fileList.concat(file);
    console.log(this.fileList);
    return false;
  };
  upload(directory, fileList) {
    const formData = new FormData();
    formData.append('currentDirectory', directory);
    //formData.append('material', this.uploadForm.get("material")?.value);
    //formData.append('effectiveDate', dateFormatter(this.addForm.get("effectiveDate")?.value));
    //formData.append('deadline', dateFormatter(this.addForm.get("deadline")?.value));
    fileList.forEach((file: any) => {
      formData.append('files', file);
    });
    this._srmMaterialService.BatchUpload(formData).subscribe(result => {
      console.log(result);
      this.fileList = [];
      this.uploading = false;
      this._messageService.success("上傳成功！");
    }, error => {
      this.uploading = false;
    });
    //return this.httpClient.post(this.uriConstant.FileUri, formData);
  }
  handleUpload() {
    for (const i in this.uploadForm.controls) {
      this.uploadForm.controls[i].markAsDirty();
      this.uploadForm.controls[i].updateValueAndValidity();
    }
    if (this.uploadForm.valid) {
      this.uploading = true;
      this.upload(this.currentDirectory, this.fileList);
    }
  }

}
