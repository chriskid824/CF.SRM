import { Component, OnInit } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { SrmDisscussionService } from 'src/app/business/srm/srm-disscussion.service';
import { ActivatedRoute } from '@angular/router';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { StorageService } from '../../../services/storage.service';
@Component({
  selector: 'app-discussion',
  templateUrl: './discussion.component.html',
  styleUrls: ['./discussion.component.less']
})
export class DiscussionComponent implements OnInit {
  public Editor = ClassicEditor;
  functions=[{id:1,name:"供應商"},{id:2,name:"料號"},{id:3,name:"詢價單"},{id:4,name:"報價單"},{id:5,name:"價格資訊"},{id:6,name:"資訊紀錄"},{id:7,name:"採購單"},{id:8,name:"出貨單"}];
  editorData:string;
  discussionId:string;
  listOfData:any;
  numbers:any;
  page: number = 1;
  size: number = 6;
  total: number;
  username:string;
  editorConfig: AngularEditorConfig = {
    editable: true,
      spellcheck: true,
      height: 'auto',
      minHeight: '200px',
      maxHeight: 'auto',
      width: 'auto',
      minWidth: '0',
      translate: 'yes',
      enableToolbar: true,
      showToolbar: true,
      placeholder: 'Enter text here...',
      defaultParagraphSeparator: '',
      defaultFontName: '',
      defaultFontSize: '',
      fonts: [
        {class: 'arial', name: 'Arial'},
        {class: 'times-new-roman', name: 'Times New Roman'},
        {class: 'calibri', name: 'Calibri'},
        {class: 'comic-sans-ms', name: 'Comic Sans MS'}
      ],
      customClasses: [
      {
        name: 'quote',
        class: 'quote',
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: 'titleText',
        class: 'titleText',
        tag: 'h1',
      },
    ],
    uploadUrl: 'http://localhost:5000/api/SrmFile/UploadFile',
    //upload: (file: File) => {  alert(file);return ""; },
    uploadWithCredentials: false,
    sanitize: true,
    toolbarPosition: 'top',
    toolbarHiddenButtons: [
      ['bold', 'italic'],
      ['fontSize']
    ]
};
  constructor(private _srmDisscussionService: SrmDisscussionService,private route: ActivatedRoute,private _storageService: StorageService, ) { }

  ngOnInit(): void {
    this.discussionId=this.route.snapshot.paramMap.get('discussionId');
    this.username=this._storageService.userName;
    this.refresh();
  }
  refresh()
  {
    var query = {
      id: this.discussionId,
      page: this.page,
      size: this.size
    }
    this._srmDisscussionService.GetDisscussion(query)
    .subscribe((result) => {
      console.info(result);
      this.listOfData=result["data"];
      this.total=result["count"];
      this.editorData=null;
    });
  }
  save() {
    if(this.editorData==undefined)
    {
      alert("內容不可為空!");
      return;
    }
    this._srmDisscussionService.AddContent({disscussionId:this.listOfData.disscussionId,disscustionContent:this.editorData}).subscribe((result: any) => {
      if(result==null)
      {
        alert("新增成功!");
        this.refresh();
      }
      else
      {
        alert(result);
      }
      //this.numbers=result;
    });
  }
  cancelEdit(){
    window.location.href='/srm/diss-list';
  }
  functionChanged(value){
    if(value==null)
    {this.numbers=null; this.listOfData.number=null;return;}
    this._srmDisscussionService.GetNumberList(value).subscribe((result: any) => {
      console.info(result);
      this.numbers=result;
    });
   }
   onEditClick(data) {
    data.isEdit=true;
  }
  onDeleteClick(data) {
    this._srmDisscussionService.DeleteContent({disscussionId:data.disscussionId,disscussionIdC:data.disscussionIdC}).subscribe((result: any) => {
      if(result==null)
      {
        alert("刪除成功!");
        this.refresh();
      }
      else
      {
        alert(result);
      }
    });
  }
  onSaveClick(data) {
    if(data.disscustionContent==undefined)
    {
      alert("內容不可為空!");
      return;
    }
    this._srmDisscussionService.UpdateContent({disscussionId:data.disscussionId,disscussionIdC:data.disscussionIdC,disscustionContent:data.disscustionContent}).subscribe((result: any) => {
      if(result==null)
      {
        alert("修改成功!");
        this.refresh();
      }
      else
      {
        alert(result);
      }
      //this.numbers=result;
    });
  }
  onReplyClick(data) {
    this.editorData='<Div style="background-color:lightgray">'+data.disscustionContent+'</div><hr><br>';
  }
  cancel(){}
  onCancelClick(data){
    this.refresh();
  }
}
