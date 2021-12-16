import { Component, OnInit, ViewChild } from '@angular/core';
import { FileInfo } from 'src/app/pages/content-manage/model/fileInfo';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FileService } from 'src/app/business/file.service';
import { FolderService } from 'src/app/business/folder.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { SrmFileService } from 'src/app/business/srm/srm-file.service';
import { ViewSrmFileRecord } from '../model/File';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { UriConfig } from 'src/app/configs/uri-config';
import{Router} from '@angular/router'
@Component({
  selector: 'app-file-modal',
  templateUrl: './file-modal.component.html',
  styleUrls: ['./file-modal.component.less']
})
export class FileModalComponent implements OnInit {
  isUpload:boolean=true;
  folderForm: FormGroup = new FormGroup({});

  fileInfoList: FileInfo[] = [];
  uploadUrl=this.uriConstant.SrmFile+'/UploadFile';
  folderList: string[] = [];
  fileRecordList: ViewSrmFileRecord[] = [];
  ischanged:boolean=false;

  @ViewChild('uploadTitleTpl', { static: true })
  uploadTitleTpl;

  @ViewChild('uploadContentTpl', { static: true })
  uploadContentTpl;

  @ViewChild('uploadFooterTpl', { static: true })
  uploadFooterTpl;

  @ViewChild('folderContentTpl', { static: true })
  folderContentTpl;

  modal: NzModalRef;

  formData:FormData;

  fileList: any[] = [];

  uploading: boolean = false;

  currentDirectory: string = '/';
  emitfiles:any[]=[];

  soucedata:ViewSrmFileRecord;
  emptyValidator = (control: FormControl): { [key: string]: any } | null => {
    const reg = /^\s+/g;
    let value = control.value?.replace(reg, '');
    return value ? null : { 'notEmpty': true };
  };

  parentEventHandlerFunction(valueEmitted) {
    this.formData.append('fileTypeList',valueEmitted.filtType);
    console.info(valueEmitted.file);
    //if(this.emitfiles!=valueEmitted.file)
    //{
      //this.emitfiles=valueEmitted.file;
      this.appendFormData(this.formData, valueEmitted.file,'files');
    //}

    //this.formData.append('files',valueEmitted.file);
    this.ischanged=true;
    // this.options[valueEmitted.index]=valueEmitted;
    // this.options[9]=valueEmitted;
    // this.revealed=!this.revealed;
    // let el: HTMLElement = this.revealbutton.nativeElement;
    // el.click();
  }

  constructor(
    private _modalService: NzModalService,
    private _messageService: NzMessageService,
    private _fileService: FileService,
    private _folderService: FolderService,
    private _formBuilder: FormBuilder,
    private _srmFileService: SrmFileService,
    private uriConstant: UriConfig,private router:Router) {
  }

  ngOnInit(): void {
    this.refresh();
  }

  dbc(file: FileInfo) {
    if (file.isDirectory) {
      let dir = file.directory + '/' + file.fileName
      this.folderList = dir.split('/').filter(e => e);
      this.currentDirectory = '/' + this.folderList.join('/');
      this.refresh();
    }
  }

  refresh() {
    this._fileService.get(1, 200, this.currentDirectory).subscribe((result: any) => {
      this.fileInfoList = result;
    });
  }
  reloadCurrentRoute() {
    const currentUrl = this.router.url;
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => {
        this.router.navigate([currentUrl]);
    });
}
  upload(data) {
    this.isUpload=data.isUpload;
    this._srmFileService.GetFileList(data).subscribe((result: any) => {
      this.fileRecordList=result;
      if(result!=null&&result.length>0)
      {
        this.fileRecordList[0].number=data.number;
        this.fileRecordList[0].deadline=data.deadline;
        this.soucedata=this.fileRecordList[0];
      }

      console.warn(this.soucedata);
      this.formData=new FormData();
      this.formData.append('json',JSON.stringify(this.fileRecordList[0]));
      console.warn(this.soucedata);
      if(this.soucedata != undefined)
      {
        this.modal = this._modalService.create({
          nzTitle: this.uploadTitleTpl,
          nzContent: this.uploadContentTpl,
          nzFooter: this.uploadFooterTpl,
          nzMaskClosable: false,
        })
      }
  else
  {
    this._messageService.error("尚未建立檔案模板");
  }
      // const a = document.createElement('a');
      // const blob = new Blob([result], { 'type': "application/octet-stream" });
      // a.href = URL.createObjectURL(blob);
      // a.download = fileInfo.fileName;
      // a.click();
    });

  }



  handleUpload() {
    this.uploading = true;
    console.info(this.formData);
    this._srmFileService.Upload(this.formData).subscribe(result => {
          this.modal.close();
          this._messageService.success("上載完畢！");
          //this.reloadCurrentRoute();
          this.fileList = [];
          this.uploading = false;
        }, error => {
          this.uploading = false;
        });
    //
    //  this._fileService.upload(this.currentDirectory, this.fileList).subscribe(result => {
    //    this.modal.close();
    //    this._messageService.success("上載完畢！");
    //    this.refresh();
    //    this.fileList = [];
    //    this.uploading = false;
    //  }, error => {
    //    this.uploading = false;
    //  });
  }

  handleChange(info: NzUploadChangeParam): void {
    console.info(info);
    if (info.file.status !== 'uploading') {
      console.log(info.file, info.fileList);
    }
    if (info.file.status === 'done') {
      this._messageService.success(`${info.file.name} file uploaded successfully`);
    } else if (info.file.status === 'error') {
      this._messageService.error(`${info.file.name} file upload failed.`);
    }
  }

  delete(fileInfo: FileInfo) {
    this._modalService.confirm({
      nzTitle: '是否刪除?',
      nzContent: null,
      nzOnOk: () => {
        if (fileInfo.isDirectory) {
          this._folderService.deleteFolder(fileInfo.directory, fileInfo.fileName).subscribe(result => {
            this._messageService.success("刪除成功！");
            this.refresh();
          });
        } else {
          this._fileService.delete(fileInfo.fileName, fileInfo.directory).subscribe(result => {
            this._messageService.success("刪除成功！");
            this.refresh();
          });
        }
      }
    });
  }

  download(fileInfo: FileInfo) {
    this._fileService.download(fileInfo.fileName, fileInfo.directory).subscribe((result: any) => {
      const a = document.createElement('a');
      const blob = new Blob([result], { 'type': "application/octet-stream" });
      a.href = URL.createObjectURL(blob);
      a.download = fileInfo.fileName;
      a.click();
    });
  }

  createFolder() {
    this.modal = this._modalService.create({
      nzTitle: '創建檔案夾',
      nzContent: this.folderContentTpl,
      nzFooter: null
    });
    this.folderForm = this._formBuilder.group({
      folderName: [null, [Validators.required, Validators.maxLength(15),
      Validators.pattern('^[^\\\\/:\*\?""<>|]{1,120}$'), this.emptyValidator]]
    });
  }

  submit() {
    for (const i in this.folderForm.controls) {
      this.folderForm.controls[i].markAsDirty();
      this.folderForm.controls[i].updateValueAndValidity();
    }
    if (this.folderForm.valid) {
      this._folderService.addFolder(this.currentDirectory, this.folderForm.value['folderName']).subscribe(result => {
        this._messageService.success("檔案夾創建成功！");
        this.refresh();
        this.modal.close();
      });
    }
  }

  cancel() {
    this.modal.close();
  }

  navgiateFolder(index) {
    if (index == -1) {
      this.currentDirectory = '/';
      this.folderList = [];
    } else {
      this.folderList = this.folderList.slice(0, index + 1);
      this.currentDirectory = '/' + this.folderList.join('/');
    }
    this.refresh();
  }

  back() {
    if (this.folderList.length >= 1) {
      this.folderList = this.folderList.slice(0, this.folderList.length - 1);
      this.currentDirectory = '/' + this.folderList.join('/');
      this.refresh();
    }
  }
  fileexist(data:any)
  {
    if(this.emitfiles.find(p=>p.uid==data.uid))
    {
      return true;
    }
    return false;
    alert(false);
  }

  appendFormData(formData, data, root = null) {
    root = root || '';
    if (data instanceof File) {
      if(!this.fileexist(data))
      {
        formData.append(root, data);
        this.emitfiles.push(data);
      }

    } else if (Array.isArray(data)) {
      for (var i = 0; i < data.length; i++) {
        //this.appendFormData(formData, data[i], root + '[' + i + ']');
        if (data[i] instanceof File) {
          if(!this.fileexist(data))
          {
            this.appendFormData(formData, data[i], root);
            this.emitfiles.push(data[i]);
          }
        } else {
          this.appendFormData(formData, data[i], root + '[' + i + ']');
        }
      }
    } else if (typeof data === 'object' && data) {
      for (var key in data) {
        if (data.hasOwnProperty(key)) {
          if (root === '') {
            this.appendFormData(formData, data[key], key);
          } else {
            this.appendFormData(formData, data[key], root + '.' + key);
          }
        }
      }
    } else {
      if (data !== null && typeof data !== 'undefined') {
        formData.append(root, data);
      }
    }
  }

}
