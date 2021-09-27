import { Component, OnInit, TemplateRef } from '@angular/core';
import { FileService } from 'src/app/business/file.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';
import { StorageService } from '../../../services/storage.service';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzAnchorModule } from 'ng-zorro-antd/anchor';

@Component({
  selector: 'app-material-trend',
  templateUrl: './material-trend.component.html',
  styleUrls: ['./material-trend.component.less']
})
export class MaterialTrendComponent implements OnInit {
  addForm: FormGroup = new FormGroup({});
  searchForm: FormGroup = new FormGroup({});
  fileList: any[] = [];
  uploading: boolean = false;
  currentDirectory: string = 'D:/CF.SRM/src/Convience.Web/Managent/src/assets/material-trend';
  tplModal: NzModalRef;
  data = [];
  page: number = 1;
  size: number = 6;
  total: number;
  imageUrl;

  constructor(private _fileService: FileService,
    private _srmMaterialService: SrmMaterialService,
    private _storageService: StorageService,
    private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _messageService: NzMessageService  ) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      material: [null, [Validators.required]],
      searchDate: [null, [Validators.required]],
    });
    this.initaddForm();
  }

  initaddForm() {
    this.addForm = this._formBuilder.group({
      material: [null, [Validators.required]],
      effectiveDate: [null, [Validators.required]],
      deadline: [null, [Validators.required]],
    });
    this.fileList = [];
  }

  open(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.initaddForm();
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false,
    });
  }

  openImg(title: TemplateRef<{}>, content: TemplateRef<{}>, imageUrl) {
    console.log(imageUrl);
    this.imageUrl = imageUrl;
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false,
    });
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
    formData.append('material', this.addForm.get("material")?.value);
    formData.append('effectiveDate', dateFormatter(this.addForm.get("effectiveDate")?.value));
    formData.append('deadline', dateFormatter(this.addForm.get("deadline")?.value));
    fileList.forEach((file: any) => {
      formData.append('files', file);
    });
    this._srmMaterialService.UploadFile(formData).subscribe(result => {
      console.log(result);
      this.fileList = [];
      this.uploading = false;
      this._messageService.success("保存成功！");
      this.tplModal.close();
    }, error => {
      this.uploading = false;
    });
    //return this.httpClient.post(this.uriConstant.FileUri, formData);
  }

  handleUpload() {
    for (const i in this.addForm.controls) {
      this.addForm.controls[i].markAsDirty();
      this.addForm.controls[i].updateValueAndValidity();
    }
    if (this.addForm.valid) {
      this.uploading = true;
      this.upload(this.currentDirectory, this.fileList);
    }
  }

  cancelEdit() {
    this.tplModal.close();
  }

  refresh() {
    for (const i in this.searchForm.controls) {
      this.searchForm.controls[i].markAsDirty();
      this.searchForm.controls[i].updateValueAndValidity();
    }
    if (this.searchForm.valid) {
      var query = {
        searchDate: dateFormatter(this.searchForm.get('searchDate').value),
        materialTrend: {
          Material: this.searchForm.get('material').value,
        },
        page: this.page,
        size: this.size
      }
      this._srmMaterialService.GetMaterialTrendList(query).subscribe(result => {
        console.log(result);
        this.data = result["data"];
        this.total = result["count"];
      });
    }
  }
  pageChange() {
    this.refresh();
  }
  sizeChange() {
    this.page = 1;
    this.refresh();
  }
  submitSearch() {
    this.page = 1;
    this.refresh();
  }
}
function dateFormatter(data) {
  if (!data) return "";
  var date = new Date(data);
  return `${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`;
}
