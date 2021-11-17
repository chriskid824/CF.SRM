import { Component, OnInit } from '@angular/core';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { SrmDisscussionService } from 'src/app/business/srm/srm-disscussion.service';
@Component({
  selector: 'app-discussion-add',
  templateUrl: './discussion-add.component.html',
  styleUrls: ['./discussion-add.component.less']
})
export class DiscussionAddComponent implements OnInit {
  public Editor = ClassicEditor;
  functions=[{id:1,name:"供應商"},{id:2,name:"料號"},{id:3,name:"詢價單"},{id:4,name:"報價單"},{id:5,name:"價格資訊"},{id:6,name:"資訊紀錄"},{id:7,name:"採購單"},{id:8,name:"出貨單"}];
  editorData:string;
  title:string;
  selectedFunctionId:number;
  numbers:any;
  selectedNumber:string;
  constructor(private _srmDisscussionService: SrmDisscussionService,) { }

  ngOnInit(): void {
  }
  save() {
    if(this.title==undefined)
    {
      alert("標題不可為空!");
      return;
    }
    if(this.editorData==undefined)
    {
      alert("內容不可為空!");
      return;
    }
    this._srmDisscussionService.AddTitle({title:this.title,content:this.editorData,templateType:this.selectedFunctionId,number:this.selectedNumber}).subscribe((result: any) => {
      if(result==null)
      {
        alert("新增成功!");
        window.location.href='/srm/diss-list';
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
    {this.numbers=null; this.selectedNumber=null;return;}
    this._srmDisscussionService.GetNumberList(value).subscribe((result: any) => {
      console.info(result);
      this.numbers=result;
    });
   }
}
