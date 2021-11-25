import { Component, OnInit, Input,EventEmitter,Output,OnChanges, SimpleChanges } from '@angular/core';
import { FileEmit, ViewSrmFileRecord } from '../model/File';
import { FileService } from 'src/app/business/file.service';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { SrmFileService } from 'src/app/business/srm/srm-file.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
@Component({
  selector: 'app-file-modal-filetype',
  templateUrl: './file-modal-filetype.component.html',
  styleUrls: ['./file-modal-filetype.component.less']
})
export class FileModalFiletypeComponent implements OnInit {
  @Output()
  buttonClicked:EventEmitter<FileEmit>=new EventEmitter<FileEmit>();
    @Input() inputFile: ViewSrmFileRecord;
    @Input() isUpload: boolean;
  constructor(    private _fileService: FileService,
                  private _srmFileService: SrmFileService,
                  private _modalService: NzModalService,
                  private _messageService: NzMessageService,) { }
  currentDirectory: string = '/';
  enable=false;
  ngOnInit(): void {
    console.info(this.isUpload);
    console.info(this.inputFile);
    this.currentDirectory=this.inputFile.werks.toString()+'/'+this.inputFile.number+'/'+ (this.inputFile.type==1? "廠內" : "廠外");
   // this.refresh();
  }
  refresh() {
    this._fileService.get(1, 200, this.currentDirectory).subscribe((result: any) => {
      //this.inputFile.fileList = result;
      console.info(this.currentDirectory);
      console.info(result);
      this.inputFile.fileList= [
        {
          uid: '12',
          name: 'xxx.png',
          status: 'done',

        }];
    });

  }
  beforeUpload = (singleFile: File, fileList: File[]): boolean => {
    this.inputFile.fileList=fileList;
    const formData = new FormData();
    const emit =new FileEmit();
    emit.filtType=this.inputFile.filetype;
    emit.file=singleFile;
    formData.append('json',JSON.stringify(this.inputFile));
    formData.append('filtType',this.inputFile.filetype);
    formData.append('files',singleFile);
    this.buttonClicked.emit(emit);
    return false;
  };

  download= (file: NzUploadFile): void => {
    console.info(file);
     this._srmFileService.download(file.uid,file.name).subscribe((result: any) => {
       const a = document.createElement('a');
       const blob = new Blob([result], { 'type': "application/octet-stream" });
       a.href = URL.createObjectURL(blob);
       a.download = file.name;
       a.click();
     });
  }

  delete=(file: NzUploadFile):void=> {
    this._modalService.confirm({
      nzTitle: '是否刪除?',
      nzContent: null,
      nzOnOk: () => {
          this._srmFileService.delete(file.uid).subscribe(result => {
            this._messageService.success("刪除成功！");
            this.inputFile.fileList=[];
            //this.refresh();
          });

      }
    });
  }
}
